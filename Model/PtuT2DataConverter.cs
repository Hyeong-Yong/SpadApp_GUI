using System.Diagnostics;
using System.Globalization;
using System.Text;

namespace SpadApp.Model
{
    /// <summary>
    /// 파일 변환 진행 상황을 UI로 전달하기 위한 이벤트 인자 클래스
    /// </summary>
    public class PtuProgressEventArgs : EventArgs
    {
        public double ProgressPercentage { get; }
        public double CpuUsage { get; }
        public long CurrentRecord { get; }
        public long TotalRecords { get; }

        public PtuProgressEventArgs(double progressPercentage, double cpuUsage, long currentRecord, long totalRecords)
        {
            ProgressPercentage = progressPercentage;
            CpuUsage = cpuUsage;
            CurrentRecord = currentRecord;
            TotalRecords = totalRecords;
        }
    }

    /// <summary>
    /// 변환 작업이 모두 끝난 후 최종 요약 리포트를 전달하기 위한 클래스
    /// </summary>
    public class PtuConversionReport
    {
        public double TotalTimeSeconds { get; set; }
        public double CpuTimeSeconds { get; set; }
        public double AverageCpuUsage { get; set; }
    }

    /// <summary>
    /// PicoHarp 300 TTTR (T2 모드) PTU 파일을 분석하여 가공 데이터를 출력하는 클래스입니다.
    /// </summary>
    public class PtuT2DataConverter
    {
        private const uint rtPicoHarpT2 = 0x00010203;
        private const int T2WRAPAROUND = 210698240;

        // WinForm UI와 비동기로 통신하기 위한 이벤트 헨들러
        public event EventHandler<PtuProgressEventArgs>? ConversionProgressChanged;
        public event EventHandler<string>? StatusMessageReceived;

        public async Task<double[]?> AnalyzeAutocorrelationAsync(string inputPtu, int targetChannel, double maxDelayNs, double binWidthNs)
        {
            if (!File.Exists(inputPtu))
            {
                OnStatusMessageReceived("오류: 대상 PTU 파일이 존재하지 않습니다.");
                return null;
            }

            return await Task.Run(() =>
            {
                try
                {
                    Process currentProcess = Process.GetCurrentProcess();
                    Stopwatch swTime = new Stopwatch();

                    int binCount = (int)(maxDelayNs / binWidthNs);
                    double[] histogram = new double[binCount];

                    using (var fs = new FileStream(inputPtu, FileMode.Open, FileAccess.Read, FileShare.Read, 1024 * 1024))
                    using (var br = new BinaryReader(fs))
                    {
                        double resolution = 4e-12;
                        long numRecords = 0;

                        // 1. 헤더 파싱 (기존 로직 동일)
                        while (true)
                        {
                            string ident = Encoding.ASCII.GetString(br.ReadBytes(32)).TrimEnd('\0');
                            int idx = br.ReadInt32();
                            uint typ = br.ReadUInt32();
                            long tagValue = br.ReadInt64();

                            if (ident == "TTResult_NumberOfRecords") numRecords = tagValue;
                            if (ident == "MeasDesc_GlobalResolution") resolution = BitConverter.Int64BitsToDouble(tagValue);
                            if (typ == 0x4001FFFF || typ == 0x4002FFFF) br.ReadBytes((int)tagValue);
                            if (ident == "Header_End") break;
                        }

                        if (numRecords == 0)
                        {
                            OnStatusMessageReceived("경고: 파일에 기록된 레코드가 0개입니다.");
                            return histogram;
                        }

                        OnStatusMessageReceived($"오프라인 Autocorrelation 분석 시작 (총 레코드: {numRecords:N0})");
                        swTime.Start();
                        TimeSpan startCpuTime = currentProcess.TotalProcessorTime;

                        // 2. 오프라인 링 버퍼 세팅 (RAM 폭발 방지)
                        const int HISTORY_SIZE = 512; // 오프라인이므로 조금 더 깊게(512) 탐색
                        long[] photonHistory = new long[HISTORY_SIZE];
                        int historyHead = 0;
                        int historyCount = 0;

                        long oflCorrection = 0;
                        double resNs = resolution * 1e9; // 초(s) 단위를 나노초(ns)로 변환

                        // 3. 파일 전체 데이터 고속 스캔
                        for (long i = 0; i < numRecords; i++)
                        {
                            uint record = br.ReadUInt32();
                            uint channel = (record >> 28) & 0xF;
                            uint time = record & 0x0FFFFFFF;

                            if (channel == 0xF)
                            {
                                if ((time & 0xF) == 0) oflCorrection += T2WRAPAROUND;
                            }
                            else if (channel == targetChannel)
                            {
                                long absoluteTicks = oflCorrection + time;

                                for (int h = 0; h < historyCount; h++)
                                {
                                    int historyIdx = (historyHead - 1 - h + HISTORY_SIZE) % HISTORY_SIZE;
                                    long pastTicks = photonHistory[historyIdx];
                                    long deltaTicks = absoluteTicks - pastTicks;

                                    if (deltaTicks <= 0) continue;

                                    double deltaNs = deltaTicks * resNs;
                                    if (deltaNs < maxDelayNs)
                                    {
                                        histogram[(int)(deltaNs / binWidthNs)]++;
                                    }
                                    else
                                    {
                                        break; // 윈도우 초과 시 즉시 이탈 (핵심 최적화)
                                    }
                                }

                                photonHistory[historyHead] = absoluteTicks;
                                historyHead = (historyHead + 1) % HISTORY_SIZE;
                                if (historyCount < HISTORY_SIZE) historyCount++;
                            }

                            // 진행률 업데이트 (UI 스레드 부하를 막기 위해 10만 번마다 한 번씩 발송)
                            if (i % 100000 == 0 && i > 0)
                            {
                                TimeSpan currentCpuTime = currentProcess.TotalProcessorTime - startCpuTime;
                                double cpuUsage = (currentCpuTime.TotalMilliseconds / swTime.Elapsed.TotalMilliseconds) * 100;
                                double progressPercent = (i * 100.0) / numRecords;

                                OnConversionProgressChanged(progressPercent, cpuUsage, i, numRecords);
                            }
                        }

                        swTime.Stop();
                        OnConversionProgressChanged(100.0, 0, numRecords, numRecords); // 100% 완료 보고
                        OnStatusMessageReceived($"분석 완료! (소요 시간: {swTime.Elapsed.TotalSeconds:F2}초)");

                        return histogram;
                    }
                }
                catch (Exception ex)
                {
                    OnStatusMessageReceived($"분석 중 오류 발생: {ex.Message}");
                    return null;
                }
            });
        }





        /// <summary>
        /// 백그라운드 스레드에서 PTU 파일을 읽어 TXT(US) 파일로 비동기 변환 처리를 수행합니다.
        /// </summary>
        /// <param name="inputPtu">입력 PTU 파일 절대/상대 경로</param>
        /// <param name="outputTxt">출력 대상 TXT 파일 절대/상대 경로</param>
        /// <returns>작업 통계 리포트 객체 (실패 시 null 반환)</returns>
        public async Task<PtuConversionReport?> ConvertPtuToTxtAsync(string inputPtu, string outputTxt)
        {
            if (!File.Exists(inputPtu))
            {
                OnStatusMessageReceived("오류: 입력 대상 PTU 파일이 존재하지 않습니다.");
                return null;
            }

            // UI 스레드가 멈추지 않도록 Task.Run을 사용하여 백그라운드 스레드로 연산 위임
            return await Task.Run(() =>
            {
                try
                {
                    Process currentProcess = Process.GetCurrentProcess();
                    Stopwatch swTime = new Stopwatch();

                    // 대용량 데이터 스트리밍 성능 최적화를 위해 내부 버퍼 크기를 1MB로 설정
                    using (var fs = new FileStream(inputPtu, FileMode.Open, FileAccess.Read, FileShare.Read, 1024 * 1024))
                    using (var br = new BinaryReader(fs))
                    using (var sw = new StreamWriter(outputTxt, false, Encoding.ASCII, 1024 * 1024))
                    {
                        double resolution = 4e-12;
                        long numRecords = 0;

                        OnStatusMessageReceived("=== 헤더 파일 파싱 시작 ===");

                        // 1. PTU 고유 헤더 블록 분석
                        while (true)
                        {
                            string ident = Encoding.ASCII.GetString(br.ReadBytes(32)).TrimEnd('\0');
                            int idx = br.ReadInt32();
                            uint typ = br.ReadUInt32();
                            long tagValue = br.ReadInt64();

                            if (ident == "TTResult_NumberOfRecords") numRecords = tagValue;
                            if (ident == "MeasDesc_GlobalResolution") resolution = BitConverter.Int64BitsToDouble(tagValue) * 1_000_000;
                            if (typ == 0x4001FFFF || typ == 0x4002FFFF) br.ReadBytes((int)tagValue);
                            if (ident == "Header_End") break;
                        }

                        OnStatusMessageReceived($"=== 파일 분석 변환 시작 (총 레코드 수: {numRecords:N0}) ===");

                        swTime.Start();
                        TimeSpan startCpuTime = currentProcess.TotalProcessorTime;

                        // 2. 대용량 T2 레코드 비트 언패킹 루프
                        long oflCorrection = 0;
                        StringBuilder sb = new StringBuilder();

                        for (long i = 0; i < numRecords; i++)
                        {
                            uint record = br.ReadUInt32();
                            uint channel = (record >> 28) & 0xF;
                            uint time = record & 0x0FFFFFFF;

                            if (channel == 0xF)
                            {
                                // 오버플로우(타이머 랩어라운드) 레코드 보정 처리
                                if ((time & 0xF) == 0) oflCorrection += T2WRAPAROUND;
                            }
                            else
                            {
                                long absoluteTimeTicks = oflCorrection + time;
                                double absoluteTimeMicro = absoluteTimeTicks * resolution;

                                sb.Append(i).Append(" CHN ")
                                  .Append(channel).Append(" ")
                                  .Append(absoluteTimeTicks).Append(" ")
                                  .Append(absoluteTimeMicro.ToString("F6", CultureInfo.InvariantCulture))
                                  .Append("\n");
                            }

                            // 매 50,000 레코드마다 스트링 빌더를 비우고 디스크에 기록하며 UI에 진척도 보고
                            if (i % 50000 == 0 && i > 0)
                            {
                                sw.Write(sb.ToString());
                                sb.Clear();

                                // 실시간 CPU 사용량 계산
                                TimeSpan currentCpuTime = currentProcess.TotalProcessorTime - startCpuTime;
                                double cpuUsage = (currentCpuTime.TotalMilliseconds / swTime.Elapsed.TotalMilliseconds) * 100;
                                double progressPercent = (i * 100.0) / numRecords;

                                // WinForm 크로스스레드 안전용 이벤트 발행
                                OnConversionProgressChanged(progressPercent, cpuUsage, i, numRecords);
                            }
                        }

                        // 루프 완료 후 버퍼에 남아있는 데이터 파일 최종 출력
                        sw.Write(sb.ToString());
                        swTime.Stop();

                        TimeSpan totalCpuTime = currentProcess.TotalProcessorTime - startCpuTime;
                        double finalCpuUsage = (totalCpuTime.TotalMilliseconds / swTime.Elapsed.TotalMilliseconds) * 100;

                        OnStatusMessageReceived("=== 데이터 변환 프로세스 성공 완료 ===");

                        return new PtuConversionReport
                        {
                            TotalTimeSeconds = swTime.Elapsed.TotalSeconds,
                            CpuTimeSeconds = totalCpuTime.TotalSeconds,
                            AverageCpuUsage = finalCpuUsage
                        };
                    }
                }
                catch (Exception ex)
                {
                    OnStatusMessageReceived($"오류 발생: {ex.Message}");
                    return null;
                }
            });
        }

        private void OnConversionProgressChanged(double percent, double cpu, long current, long total)
        {
            ConversionProgressChanged?.Invoke(this, new PtuProgressEventArgs(percent, cpu, current, total));
        }

        private void OnStatusMessageReceived(string message)
        {
            StatusMessageReceived?.Invoke(this, message);
        }
    }
}