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

        /// <summary>
        /// [고해상도 오프라인 분석] 짧은 수명(Trap Lifetime 등)을 가진 소규모 데이터의 
        /// 정밀 자기상관 분석을 수행합니다. (예: MaxDelay = 10us, BinWidth = 10ns)
        /// </summary>
        public async Task<double[]?> AnalyzeHighResAutocorrelationAsync(
            string inputPtu, double maxDelayNs = 10000, double binWidthNs = 10)
        {
            if (!File.Exists(inputPtu)) return null;

            return await Task.Run(() =>
            {
                try
                {
                    Stopwatch sw = new Stopwatch();
                    sw.Start();

                    // 1. 5만 개 데이터를 담을 리스트 (5만 개면 RAM 점유율 1MB도 안 됨)
                    List<long> absoluteTicks = new List<long>();
                    double resolutionPs = 4.0;
                    long oflCorrection = 0;

                    // 파일 읽기
                    using (var fs = new FileStream(inputPtu, FileMode.Open, FileAccess.Read))
                    using (var br = new BinaryReader(fs))
                    {
                        LogMessage("고해상도 분석: 데이터 로드 중...");

                        while (true) // 헤더 건너뛰기
                        {
                            string ident = Encoding.ASCII.GetString(br.ReadBytes(32)).TrimEnd('\0');
                            uint typ = br.ReadUInt32();
                            br.ReadInt32(); // idx
                            long tagValue = br.ReadInt64();

                            if (ident == "MeasDesc_GlobalResolution") resolutionPs = BitConverter.Int64BitsToDouble(tagValue) * 1e12;
                            if (typ == 0x4001FFFF || typ == 0x4002FFFF) br.ReadBytes((int)tagValue);
                            if (ident == "Header_End") break;
                        }

                        // 이진 데이터 파싱
                        while (br.BaseStream.Position < br.BaseStream.Length)
                        {
                            uint record = br.ReadUInt32();
                            uint channel = (record >> 28) & 0xF;
                            uint time = record & 0x0FFFFFFF;

                            if (channel == 0xF)
                            {
                                if ((time & 0xF) == 0) oflCorrection += T2WRAPAROUND;
                            }
                            else if (channel == TargetChannel)
                            {
                                absoluteTicks.Add(oflCorrection + time);
                            }
                        }
                    }

                    int count = absoluteTicks.Count;
                    if (count == 0) return null;

                    LogMessage($"고해상도 분석: {count:N0}개 광자 로드 완료. 상관도 계산 시작...");

                    // 2. 고해상도 히스토그램 생성
                    double resNs = resolutionPs / 1000.0;
                    int binCount = (int)(maxDelayNs / binWidthNs);
                    double[] histogram = new double[binCount];

                    // 3. 직접 자기상관 연산 (Windowed $O(N)$)
                    for (int i = 0; i < count; i++)
                    {
                        long baseTick = absoluteTicks[i];

                        // 자기 자신 이후의 광자들과 거리 비교
                        for (int j = i + 1; j < count; j++)
                        {
                            long delta = absoluteTicks[j] - baseTick;
                            double deltaNs = delta * resNs;

                            // 우리가 관심있는 윈도우(예: 10us)를 넘어가면 즉시 내부 루프 탈출 (엄청난 속도 향상)
                            if (deltaNs >= maxDelayNs) break;

                            int binIdx = (int)(deltaNs / binWidthNs);
                            histogram[binIdx]++;
                        }
                    }

                    sw.Stop();
                    LogMessage($"고해상도 분석 완료 (소요 시간: {sw.Elapsed.TotalSeconds:F3}초)");

                    return histogram;
                }
                catch (Exception ex)
                {
                    LogMessage($"[오류] 고해상도 분석 실패: {ex.Message}");
                    return null;
                }
            });
        }


        public async Task<(double[] delays, double[] correlations)?> AnalyzeMultiTauAsync(
            string inputPtu, double baseBinWidthNs = 1000, int cascades = 20, int binsPerCascade = 16)
        {
            if (!File.Exists(inputPtu)) return null;

            // ★ 해결: Task.Run 뒤에 꺾쇠 < > 를 사용하여 반환 타입을 명확하게 알려줍니다.
            return await Task.Run<(double[] delays, double[] correlations)?>(() =>
            {
                try
                {
                    Stopwatch sw = new Stopwatch();
                    sw.Start();

                    List<long> targetTicks = new List<long>();
                    double resolutionPs = 4.0;
                    long numRecords = 0;
                    long oflCorrection = 0;

                    using (var fs = new FileStream(inputPtu, FileMode.Open, FileAccess.Read, FileShare.Read, 1024 * 1024))
                    using (var br = new BinaryReader(fs))
                    {
                        LogMessage("다중 타우 분석: 헤더 분석 및 데이터 로드 시작...");

                        while (true)
                        {
                            string ident = Encoding.ASCII.GetString(br.ReadBytes(32)).TrimEnd('\0');
                            int idx = br.ReadInt32();
                            uint typ = br.ReadUInt32();
                            long tagValue = br.ReadInt64();

                            if (ident == "TTResult_NumberOfRecords") numRecords = tagValue;
                            if (ident == "MeasDesc_GlobalResolution") resolutionPs = BitConverter.Int64BitsToDouble(tagValue) * 1e12;
                            if (typ == 0x4001FFFF || typ == 0x4002FFFF) br.ReadBytes((int)tagValue);
                            if (ident == "Header_End") break;
                        }

                        for (long i = 0; i < numRecords; i++)
                        {
                            uint record = br.ReadUInt32();
                            uint channel = (record >> 28) & 0xF;
                            uint time = record & 0x0FFFFFFF;

                            if (channel == 0xF)
                            {
                                if ((time & 0xF) == 0) oflCorrection += T2WRAPAROUND;
                            }
                            else if (channel == TargetChannel)
                            {
                                targetTicks.Add(oflCorrection + time);
                            }
                        }
                    }

                    if (targetTicks.Count == 0) return null;

                    LogMessage($"다중 타우 분석: {targetTicks.Count:N0}개 광자 Binning 시작...");

                    double ticksPerBaseBin = (baseBinWidthNs * 1000.0) / resolutionPs;
                    long maxTick = targetTicks[targetTicks.Count - 1];
                    int numBaseBins = (int)(maxTick / ticksPerBaseBin) + 1;

                    double[] currentTrace = new double[numBaseBins];
                    foreach (var tick in targetTicks)
                    {
                        int binIdx = (int)(tick / ticksPerBaseBin);
                        if (binIdx < numBaseBins) currentTrace[binIdx]++;
                    }

                    targetTicks.Clear();
                    targetTicks.TrimExcess();

                    LogMessage($"다중 타우 분석: {numBaseBins:N0}개 Bin에 대한 상관 연산 진행 중...");

                    List<double> delays = new List<double>();
                    List<double> correlations = new List<double>();
                    double currentBinNs = baseBinWidthNs;

                    for (int cascade = 0; cascade < cascades; cascade++)
                    {
                        int startLag = (cascade == 0) ? 1 : binsPerCascade / 2;
                        int endLag = binsPerCascade;

                        for (int m = startLag; m < endLag; m++)
                        {
                            if (m >= currentTrace.Length) break;

                            double sumProduct = 0, sumI = 0, sumDelayed = 0;
                            int N = currentTrace.Length - m;

                            for (int i = 0; i < N; i++)
                            {
                                sumProduct += currentTrace[i] * currentTrace[i + m];
                                sumI += currentTrace[i];
                                sumDelayed += currentTrace[i + m];
                            }

                            double meanI = sumI / N;
                            double meanDelayed = sumDelayed / N;
                            double g = 0;

                            if (meanI > 0 && meanDelayed > 0)
                            {
                                g = (sumProduct / N) / (meanI * meanDelayed) - 1.0;
                            }

                            delays.Add(m * currentBinNs);
                            correlations.Add(g);
                        }

                        int nextLength = currentTrace.Length / 2;
                        if (nextLength <= binsPerCascade) break;

                        double[] nextTrace = new double[nextLength];
                        for (int i = 0; i < nextLength; i++)
                        {
                            nextTrace[i] = currentTrace[2 * i] + currentTrace[2 * i + 1];
                        }

                        currentTrace = nextTrace;
                        currentBinNs *= 2;
                    }

                    sw.Stop();
                    LogMessage($"다중 타우 분석 완료 (소요 시간: {sw.Elapsed.TotalSeconds:F2}초)");

                    return (delays.ToArray(), correlations.ToArray());
                }
                catch (Exception ex)
                {
                    LogMessage($"[오류] 다중 타우 분석 실패: {ex.Message}");
                    return null;
                }
            });
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