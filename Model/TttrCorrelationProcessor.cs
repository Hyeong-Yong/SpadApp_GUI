using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SpadApp.Controller;

namespace SpadApp.Model
{
    public class TttrProgressEventArgs : EventArgs
    {
        public double ElapsedSeconds { get; }
        public long TotalRecordsReceived { get; }
        public TttrProgressEventArgs(double elapsedSeconds, long totalRecords)
        {
            ElapsedSeconds = elapsedSeconds;
            TotalRecordsReceived = totalRecords;
        }
    }

    // ★ [핵심 추가] 두 가지 분석 모드를 구분하기 위한 열거형
    public enum CorrelationMode
    {
        InterArrival,    // 고속 모니터링 (Nearest-Neighbor)
        Autocorrelation  // 정밀 측정 (Full-Pair 링 버퍼)
    }

    /// <summary>
    /// TTTR (T2/T3) 모드에서 실시간 FIFO 데이터를 RAM으로 직결하여 
    /// 시간 상관 분석 및 초고속 디스크 스트리밍(.ptu)을 수행하는 프로세서입니다.
    /// </summary>
    public class TttrCorrelationProcessor
    {
        public int ReferenceChannel { get; set; } = 1;
        public int TargetChannel { get; set; } = 1;

        // ★ [핵심 추가] 모드 및 파일 저장 관련 프로퍼티
        public CorrelationMode Mode { get; set; } = CorrelationMode.InterArrival;
        public bool EnableDataSaving { get; set; } = false;
        public string SaveFilePath { get; set; } = "";

        private const int T2WRAPAROUND = 210698240;
        private const int FIFO_CHUNK_SIZE = 131072;

        // ★ [핵심 추가] Autocorrelation용 링 버퍼 설정
        private const int HISTORY_SIZE = 256;

        private readonly DeviceController_PicoHarp300 _tcspcController;

        public event EventHandler<TttrProgressEventArgs>? ProgressUpdated;
        public event EventHandler<string>? StatusMessageLogged;
        public event EventHandler? MeasurementFinished;

        public double[] CorrelationHistogram { get; }
        public double BinWidthNs { get; }
        public double MaxDelayNs { get; }

        private volatile bool _isMeasuring = false;
        private long _totalRecords = 0;

        public TttrCorrelationProcessor(DeviceController_PicoHarp300 tcspcController, double maxDelayNs = 10000, double binWidthNs = 1)
        {
            _tcspcController = tcspcController;
            BinWidthNs = binWidthNs;
            MaxDelayNs = maxDelayNs;
            int binCount = (int)(maxDelayNs / binWidthNs);
            CorrelationHistogram = new double[binCount];
        }

        public async Task StartAcquisitionAsync(int targetDurationMs)
        {
            _isMeasuring = true;
            _totalRecords = 0;
            Array.Clear(CorrelationHistogram, 0, CorrelationHistogram.Length);

            LogMessage($"TTTR 분석 가동 (Mode: {Mode}, Save: {EnableDataSaving})");

            await Task.Run(() =>
            {
                uint[] fifoBuffer = new uint[FIFO_CHUNK_SIZE];
                long oflCorrection = 0;

                // [InterArrival 용 변수]
                long[] lastPhotonTicks = new long[16];
                for (int i = 0; i < 16; i++) lastPhotonTicks[i] = -1;

                // [Autocorrelation 용 변수]
                long[] photonHistory = new long[HISTORY_SIZE];
                int historyHead = 0;
                int historyCount = 0;

                double resolutionPs = _tcspcController.GetResolutionPs();
                double resNs = resolutionPs / 1000.0;
                int histogramLength = CorrelationHistogram.Length;

                // ★ [파일 스트리밍 용 객체]
                FileStream? fs = null;
                BinaryWriter? bw = null;
                long recordCountPos = 0;

                try
                {
                    // 1. 파일 저장 옵션이 켜져있으면 임시 헤더 작성 (Mock Header)
                    if (EnableDataSaving && !string.IsNullOrEmpty(SaveFilePath))
                    {
                        fs = new FileStream(SaveFilePath, FileMode.Create, FileAccess.Write, FileShare.None, 1024 * 1024);
                        bw = new BinaryWriter(fs);

                        // 태그 1: TTResult_NumberOfRecords (임시값 0 기록 및 위치 기억)
                        bw.Write(Encoding.ASCII.GetBytes("TTResult_NumberOfRecords".PadRight(32, '\0')));
                        bw.Write((int)0);
                        bw.Write((uint)0x10000008); // tyInt8
                        recordCountPos = fs.Position;
                        bw.Write((long)0);

                        // 태그 2: MeasDesc_GlobalResolution (PtuT2DataConverter 규격에 맞게 Seconds로 변환)
                        bw.Write(Encoding.ASCII.GetBytes("MeasDesc_GlobalResolution".PadRight(32, '\0')));
                        bw.Write((int)0);
                        bw.Write((uint)0x20000008); // tyFloat8
                        double resSec = resolutionPs * 1e-12;
                        bw.Write(BitConverter.DoubleToInt64Bits(resSec));

                        // 태그 3: Header_End
                        bw.Write(Encoding.ASCII.GetBytes("Header_End".PadRight(32, '\0')));
                        bw.Write((int)0);
                        bw.Write((uint)0xFFFF0008); // tyEmpty
                        bw.Write((long)0);
                    }

                    Stopwatch sw = new Stopwatch();
                    sw.Start();
                    long nextUpdateTicks = 0;

                    // 2. 메인 FIFO 수집 루프
                    while (_isMeasuring)
                    {
                        if (sw.Elapsed.TotalMilliseconds >= targetDurationMs) break;

                        int recordsRead = _tcspcController.ReadFiFo(fifoBuffer, FIFO_CHUNK_SIZE);

                        if (recordsRead < 0)
                        {
                            LogMessage($"[오류] 통신 실패: {recordsRead}");
                            break;
                        }

                        if (recordsRead > 0)
                        {
                            // ★ [하이브리드 아키텍처 1단계] 퍼온 데이터를 연산 전에 무조건 디스크로 복사
                            if (bw != null)
                            {
                                // BlockCopy를 쓰면 uint 배열을 루프 없이 초고속으로 byte 스트림으로 디스크에 박아넣습니다.
                                byte[] byteBuffer = new byte[recordsRead * 4];
                                Buffer.BlockCopy(fifoBuffer, 0, byteBuffer, 0, recordsRead * 4);
                                bw.Write(byteBuffer);
                            }

                            _totalRecords += recordsRead;

                            // ★ [하이브리드 아키텍처 2단계] 모드에 따른 차트 연산
                            for (int i = 0; i < recordsRead; i++)
                            {
                                uint record = fifoBuffer[i];
                                uint channel = (record >> 28) & 0xF;
                                uint time = record & 0x0FFFFFFF;

                                if (channel == 0xF)
                                {
                                    if ((time & 0xF) == 0) oflCorrection += T2WRAPAROUND;
                                }
                                else if (channel < 16)
                                {
                                    long absoluteTicks = oflCorrection + time;

                                    if (channel == TargetChannel)
                                    {
                                        if (Mode == CorrelationMode.InterArrival)
                                        {
                                            // --- [고속] Inter-arrival (Nearest-Neighbor) 로직 ---
                                            long refTicks = lastPhotonTicks[ReferenceChannel];
                                            if (refTicks != -1)
                                            {
                                                long deltaTicks = absoluteTicks - refTicks;
                                                if (deltaTicks > 0)
                                                {
                                                    double deltaNs = deltaTicks * resNs;
                                                    if (deltaNs < MaxDelayNs)
                                                    {
                                                        CorrelationHistogram[(int)(deltaNs / BinWidthNs)]++;
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            // --- [정밀] Autocorrelation (Full-Pair Ring Buffer) 로직 ---
                                            for (int h = 0; h < historyCount; h++)
                                            {
                                                int idx = (historyHead - 1 - h + HISTORY_SIZE) % HISTORY_SIZE;
                                                long pastTicks = photonHistory[idx];
                                                long deltaTicks = absoluteTicks - pastTicks;

                                                if (deltaTicks <= 0) continue;

                                                double deltaNs = deltaTicks * resNs;
                                                if (deltaNs < MaxDelayNs)
                                                {
                                                    CorrelationHistogram[(int)(deltaNs / BinWidthNs)]++;
                                                }
                                                else break; // 최적화: 윈도우를 벗어나면 루프 탈출
                                            }
                                            photonHistory[historyHead] = absoluteTicks;
                                            historyHead = (historyHead + 1) % HISTORY_SIZE;
                                            if (historyCount < HISTORY_SIZE) historyCount++;
                                        }
                                    }

                                    // InterArrival 모드를 위해 마지막 틱 항상 갱신
                                    lastPhotonTicks[channel] = absoluteTicks;
                                }
                            }
                        }
                        else
                        {
                            Thread.Sleep(1);
                        }

                        if (sw.ElapsedMilliseconds > nextUpdateTicks)
                        {
                            ProgressUpdated?.Invoke(this, new TttrProgressEventArgs(sw.Elapsed.TotalSeconds, _totalRecords));
                            nextUpdateTicks = sw.ElapsedMilliseconds + 500;
                        }
                    }

                    sw.Stop();
                }
                finally
                {
                    // ★ 3. 측정 종료 시: 포인터를 돌려 총 레코드 수를 덮어쓰고 파일 닫기
                    if (bw != null && fs != null)
                    {
                        try
                        {
                            fs.Seek(recordCountPos, SeekOrigin.Begin);
                            bw.Write(_totalRecords);
                            bw.Flush();
                        }
                        finally
                        {
                            bw.Close();
                            fs.Close();
                        }
                        LogMessage($"파일 저장 완료: {_totalRecords:N0} 레코드");
                    }

                    _isMeasuring = false;
                    ProgressUpdated?.Invoke(this, new TttrProgressEventArgs(0, _totalRecords));
                    MeasurementFinished?.Invoke(this, EventArgs.Empty);
                }
            });
        }

        public void StopAcquisition()
        {
            _isMeasuring = false;
        }

        private void LogMessage(string msg)
        {
            StatusMessageLogged?.Invoke(this, $"[{DateTime.Now:HH:mm:ss}] {msg}");
        }
    }
}