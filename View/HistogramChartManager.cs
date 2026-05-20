using ScottPlot;
using ScottPlot.WinForms;
using SpadApp.DLLWrapper;
using SpadApp.Parameters;
using System;

namespace SpadApp.View
{
    public class HistogramChartManager
    {
        private readonly FormsPlot _plot;
        private ScottPlot.Plottables.Signal? _signal;

        // 동적 크기 할당을 위해 빈 배열로 초기화
        private double[] _plotData = Array.Empty<double>();

        private bool _isLogScaleY = false;
        private bool _isLogScaleX = false;

        // ★ uint[] 대신 double[]을 사용하여 어떤 데이터든 담을 수 있는 범용 버퍼로 변경
        private double[]? _lastData;
        private double _lastPeriodNs = 0.004;

        private bool _useManualXLimits = false;
        private double _manualXMin = 0;
        private double _manualXMax = 50;

        // ★ 외부(ucAPP 등)에서 차트 제목을 자유롭게 변경할 수 있도록 프로퍼티 추가
        public string ChartTitle { get; set; } = "Time-Resolved Photon Spectrum";

        public HistogramChartManager(FormsPlot plot)
        {
            _plot = plot;
            InitChart();
        }

        private void InitChart()
        {
            UpdatePlotRender();
        }

        private void UpdatePlotRender()
        {
            _plot.Plot.Clear();
            if (_lastData == null || _lastData.Length == 0) return;

            // 데이터 크기가 바뀌었으면 플롯 렌더링용 버퍼도 재할당
            if (_plotData.Length != _lastData.Length)
            {
                _plotData = new double[_lastData.Length];
            }

            if (_isLogScaleX)
            {
                int count = _lastData.Length - 1;
                double[] xs = new double[count];
                double[] ys = new double[count];

                for (int i = 1; i < _lastData.Length; i++)
                {
                    double timeNs = i * _lastPeriodNs;
                    xs[i - 1] = Math.Log10(timeNs);

                    if (_isLogScaleY)
                        ys[i - 1] = _lastData[i] > 0 ? Math.Log10(_lastData[i]) : 0;
                    else
                        ys[i - 1] = _lastData[i];
                }

                var scatter = _plot.Plot.Add.Scatter(xs, ys);
                scatter.Color = Colors.Blue;
                scatter.LineWidth = 1.5f;
                scatter.MarkerSize = 0;
            }
            else
            {
                for (int i = 0; i < _lastData.Length; i++)
                {
                    if (_isLogScaleY)
                        _plotData[i] = _lastData[i] > 0 ? Math.Log10(_lastData[i]) : 0;
                    else
                        _plotData[i] = _lastData[i];
                }

                _signal = _plot.Plot.Add.Signal(_plotData);
                _signal.Data.Period = _lastPeriodNs;
                _signal.Color = Colors.Blue;
                _signal.LineWidth = 1.5f;
            }

            ApplyAxisSettings();

            _plot.Plot.Axes.AutoScale();

            if (_useManualXLimits)
            {
                if (_isLogScaleX)
                {
                    double logMin = _manualXMin > 0 ? Math.Log10(_manualXMin) : -3;
                    double logMax = _manualXMax > 0 ? Math.Log10(_manualXMax) : 2;
                    _plot.Plot.Axes.SetLimitsX(logMin, logMax);
                }
                else
                {
                    _plot.Plot.Axes.SetLimitsX(_manualXMin, _manualXMax);
                }
            }

            if (_isLogScaleY)
            {
                var limits = _plot.Plot.Axes.GetLimits();
                _plot.Plot.Axes.SetLimitsY(0, limits.Top);
            }

            _plot.Refresh();
        }

        private void ApplyAxisSettings()
        {
            // ★ 프로퍼티를 사용하여 동적으로 제목 셋팅
            _plot.Plot.Title(ChartTitle);
            _plot.Plot.Grid.MajorLineColor = Colors.LightGray.WithAlpha(0.5);

            if (_isLogScaleY)
            {
                _plot.Plot.YLabel("Photon Counts (Log Scale)");
                _plot.Plot.Axes.Left.TickGenerator = new ScottPlot.TickGenerators.NumericAutomatic
                {
                    IntegerTicksOnly = true,
                    MinorTickGenerator = new ScottPlot.TickGenerators.LogMinorTickGenerator(),
                    LabelFormatter = (val) => val >= 0 ? $"10^{val:F0}" : "0"
                };
            }
            else
            {
                _plot.Plot.YLabel("Photon Counts (number)");
                _plot.Plot.Axes.Left.TickGenerator = new ScottPlot.TickGenerators.NumericAutomatic();
            }

            if (_isLogScaleX)
            {
                _plot.Plot.XLabel("Time Delay (Log Scale ns)");
                _plot.Plot.Axes.Bottom.TickGenerator = new ScottPlot.TickGenerators.NumericAutomatic
                {
                    MinorTickGenerator = new ScottPlot.TickGenerators.LogMinorTickGenerator(),
                    LabelFormatter = (val) => Math.Pow(10, val).ToString("0.###")
                };
            }
            else
            {
                _plot.Plot.XLabel("Time Delay (ns)");
                _plot.Plot.Axes.Bottom.TickGenerator = new ScottPlot.TickGenerators.NumericAutomatic();
            }
        }

        public void SetXLimits(double minNs, double maxNs)
        {
            _useManualXLimits = true;
            _manualXMin = minNs;
            _manualXMax = maxNs;
            UpdatePlotRender();
        }

        public void ResetXLimitsToAuto()
        {
            _useManualXLimits = false;
            UpdatePlotRender();
        }

        public bool ToggleScaleModeY()
        {
            _isLogScaleY = !_isLogScaleY;
            UpdatePlotRender();
            return _isLogScaleY;
        }

        public bool ToggleScaleModeX()
        {
            _isLogScaleX = !_isLogScaleX;
            UpdatePlotRender();
            return _isLogScaleX;
        }

        public void UpdateTimeAxis(double resolutionPs)
        {
            _lastPeriodNs = resolutionPs / 1000.0;
            UpdatePlotRender();
        }

        // =========================================================
        // 데이터 주입 메서드 오버로딩 (범용성 확장)
        // =========================================================

        /// <summary>
        /// 원본 하드웨어 데이터 (MainView 전용)
        /// </summary>
        public void UpdateFromHardware(uint[] rawData)
        {
            double resPs = 0;
            PicoHarp_Native.PH_GetResolution(PicoHarp_DeviceInfo.DeviceIndex, ref resPs);
            _lastPeriodNs = resPs / 1000.0;

            EnsureDataBuffer(rawData.Length);
            for (int i = 0; i < rawData.Length; i++) _lastData![i] = rawData[i];

            UpdatePlotRender();
        }

        public void Update(uint[] rawData, double resolutionPs)
        {
            _lastPeriodNs = resolutionPs / 1000.0;
            EnsureDataBuffer(rawData.Length);
            for (int i = 0; i < rawData.Length; i++) _lastData![i] = rawData[i];

            UpdatePlotRender();
        }

        /// <summary>
        /// ★ RAM에서 가공된 상관 분석 데이터 주입용 (ucAPP 전용)
        /// </summary>
        public void UpdateFromProcessedData(double[] processedData, double periodNs)
        {
            _lastPeriodNs = periodNs;
            EnsureDataBuffer(processedData.Length);
            Array.Copy(processedData, _lastData!, processedData.Length);

            UpdatePlotRender();
        }

        private void EnsureDataBuffer(int length)
        {
            if (_lastData == null || _lastData.Length != length)
            {
                _lastData = new double[length];
            }
        }
    }
}