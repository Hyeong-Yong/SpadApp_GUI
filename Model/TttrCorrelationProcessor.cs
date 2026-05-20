using System;
using System.Diagnostics;
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

    /// <summary>
    /// TTTR (T2/T3) 모드에서 실시간 FIFO 데이터를 RAM으로 직결하여 시간 상관 분석을 수행하는 범용 프로세서입니다.
    /// </summary>
    public class TttrCorrelationProcessor
    {

        // ★ [핵심 추가] 범용 상관 분석을 위한 채널 선택 프로퍼티
        // Afterpulse 측정 시: Ref=1, Target=1
        // Crosstalk 측정 시: Ref=1, Target=2
        public int ReferenceChannel { get; set; } = 1;
        public int TargetChannel { get; set; } = 1;

        private const int T2WRAPAROUND = 210698240;
        private const int FIFO_CHUNK_SIZE = 131072;

        private readonly DeviceController_PicoHarp300 _tcspcController;

        public event EventHandler<TttrProgressEventArgs>? ProgressUpdated;
        public event EventHandler<string>? StatusMessageLogged;
        public event EventHandler? MeasurementFinished;

        public double[] CorrelationHistogram { get; }
        public double BinWidthNs { get; }

        // ★ 스레드 안전성 보장을 위한 volatile 추가
        private volatile bool _isMeasuring = false;
        private long _totalRecords = 0;


        public TttrCorrelationProcessor(DeviceController_PicoHarp300 tcspcController, double maxDelayNs = 10000, double binWidthNs = 1)
        {
            _tcspcController = tcspcController;
            BinWidthNs = binWidthNs;
            int binCount = (int)(maxDelayNs / binWidthNs);
            CorrelationHistogram = new double[binCount];
        }

        public async Task StartAcquisitionAsync(int targetDurationMs)
        {
            _isMeasuring = true;
            _totalRecords = 0;
            Array.Clear(CorrelationHistogram, 0, CorrelationHistogram.Length);

            LogMessage($"TTTR 상관 분석 가동 (Ref CH: {ReferenceChannel} -> Target CH: {TargetChannel})");

            await Task.Run(() =>
            {
                uint[] fifoBuffer = new uint[FIFO_CHUNK_SIZE];
                long oflCorrection = 0;

                // ★ [핵심 변경] 각 채널(0~15)별로 마지막 광자 도착 시간을 기억하는 배열
                long[] lastPhotonTicks = new long[16];
                for (int i = 0; i < 16; i++) lastPhotonTicks[i] = -1;

                double resolutionPs = _tcspcController.GetResolutionPs();
                double resNs = resolutionPs / 1000.0;

                Stopwatch sw = new Stopwatch();
                sw.Start();

                long nextUpdateTicks = 0;

                while (_isMeasuring)
                {
                    double elapsedMs = sw.Elapsed.TotalMilliseconds;
                    if (elapsedMs >= targetDurationMs)
                    {
                        LogMessage($"목표 계측 시간 도달 ({targetDurationMs / 1000.0:F1}초).");
                        break;
                    }

                    int recordsRead = _tcspcController.ReadFiFo(fifoBuffer, FIFO_CHUNK_SIZE);

                    if (recordsRead < 0)
                    {
                        LogMessage($"[오류] 통신 실패 에러코드: {recordsRead}");
                        break;
                    }

                    if (recordsRead > 0)
                    {
                        _totalRecords += recordsRead;

                        for (int i = 0; i < recordsRead; i++)
                        {
                            uint record = fifoBuffer[i];
                            uint channel = (record >> 28) & 0xF;
                            uint time = record & 0x0FFFFFFF;

                            if (channel == 0xF)
                            {
                                // 오버플로우 마커
                                if ((time & 0xF) == 0) oflCorrection += T2WRAPAROUND;
                            }
                            else if (channel < 16)
                            {
                                long absoluteTicks = oflCorrection + time;

                                // ★ [핵심 분석 로직] 현재 광자가 '타겟 채널'에서 들어왔을 때만 계산!
                                if (channel == TargetChannel)
                                {
                                    long refTicks = lastPhotonTicks[ReferenceChannel];

                                    // 기준 채널(Ref)에 광자가 찍힌 적이 있다면 시간차(Delta) 계산
                                    if (refTicks != -1)
                                    {
                                        long deltaTicks = absoluteTicks - refTicks;

                                        // 0보다 큰 정상적인 지연 시간일 경우 히스토그램 누적
                                        if (deltaTicks > 0)
                                        {
                                            double deltaNs = deltaTicks * resNs;

                                            if (deltaNs < (CorrelationHistogram.Length * BinWidthNs))
                                            {
                                                int binIndex = (int)(deltaNs / BinWidthNs);
                                                CorrelationHistogram[binIndex]++;
                                            }
                                        }
                                    }
                                }

                                // 방금 들어온 광자의 도착 시간을 해당 채널 메모리에 갱신
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
                _isMeasuring = false;
                ProgressUpdated?.Invoke(this, new TttrProgressEventArgs(sw.Elapsed.TotalSeconds, _totalRecords));
                MeasurementFinished?.Invoke(this, EventArgs.Empty);
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