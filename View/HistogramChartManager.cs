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
        private double[] _plotData;

        private bool _isLogScaleY = false;
        private bool _isLogScaleX = false;
        private uint[]? _lastRawData;
        private double _lastResolutionPs = 4.0;

        // ★ X축 수동 범위 설정을 위한 필드 추가
        private bool _useManualXLimits = false;
        private double _manualXMin = 0;
        private double _manualXMax = 50;

        public HistogramChartManager(FormsPlot plot)
        {
            _plot = plot;
            _plotData = new double[PicoHarp_Native.HISTCHAN];
            InitChart();
        }

        private void InitChart()
        {
            _lastRawData = new uint[PicoHarp_Native.HISTCHAN];
            UpdatePlotRender();
        }

        private void UpdatePlotRender()
        {
            _plot.Plot.Clear();
            if (_lastRawData == null) return;

            double resNs = _lastResolutionPs / 1000.0;

            if (_isLogScaleX)
            {
                int count = _lastRawData.Length - 1;
                double[] xs = new double[count];
                double[] ys = new double[count];

                for (int i = 1; i < _lastRawData.Length; i++)
                {
                    double timeNs = i * resNs;
                    xs[i - 1] = Math.Log10(timeNs);

                    if (_isLogScaleY)
                        ys[i - 1] = _lastRawData[i] > 0 ? Math.Log10(_lastRawData[i]) : 0;
                    else
                        ys[i - 1] = _lastRawData[i];
                }

                var scatter = _plot.Plot.Add.Scatter(xs, ys);
                scatter.Color = Colors.Blue;
                scatter.LineWidth = 1.5f;
                scatter.MarkerSize = 0;
            }
            else
            {
                for (int i = 0; i < _lastRawData.Length; i++)
                {
                    if (_isLogScaleY)
                        _plotData[i] = _lastRawData[i] > 0 ? Math.Log10(_lastRawData[i]) : 0;
                    else
                        _plotData[i] = _lastRawData[i];
                }

                _signal = _plot.Plot.Add.Signal(_plotData);
                _signal.Data.Period = resNs;
                _signal.Color = Colors.Blue;
                _signal.LineWidth = 1.5f;
            }

            ApplyAxisSettings();

            // 1. 전체 범위를 기본적으로 자동 맞춤 실행
            _plot.Plot.Axes.AutoScale();

            // ★ 2. 만약 유저가 X축 수동 범위를 활성화했다면 덮어쓰기 적용
            if (_useManualXLimits)
            {
                if (_isLogScaleX)
                {
                    // X축이 로그 스케일일 경우 입력된 물리적 ns 범위값을 로그 스페이스 좌표로 변환하여 매핑
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
            _plot.Plot.Title("Time-Resolved Photon Spectrum");
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
                _plot.Plot.XLabel("Time (Log Scale ns)");
                _plot.Plot.Axes.Bottom.TickGenerator = new ScottPlot.TickGenerators.NumericAutomatic
                {
                    MinorTickGenerator = new ScottPlot.TickGenerators.LogMinorTickGenerator(),
                    LabelFormatter = (val) => Math.Pow(10, val).ToString("0.###")
                };
            }
            else
            {
                _plot.Plot.XLabel("Time (ns)");
                _plot.Plot.Axes.Bottom.TickGenerator = new ScottPlot.TickGenerators.NumericAutomatic();
            }
        }

        /// <summary>
        /// ★ 외부 UI 컨트롤(NumericUpDown)에서 수동 X축 입력을 보낼 때 호출하는 메서드입니다.
        /// </summary>
        public void SetXLimits(double minNs, double maxNs)
        {
            _useManualXLimits = true;
            _manualXMin = minNs;
            _manualXMax = maxNs;

            // 실시간 렌더링 파이프라인 가동
            UpdatePlotRender();
        }

        /// <summary>
        /// ★ 외부 UI 버튼(Reset)을 눌러 다시 전체 자동 맞춤 상태로 변환할 때 호출합니다.
        /// </summary>
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
            _lastResolutionPs = resolutionPs;
            UpdatePlotRender();
        }

        public void UpdateFromHardware(uint[] rawData)
        {
            double resPs = 0;
            PicoHarp_Native.PH_GetResolution(PicoHarp_DeviceInfo.DeviceIndex, ref resPs);
            _lastResolutionPs = resPs;

            _lastRawData = rawData;
            UpdatePlotRender();
        }

        public void Update(uint[] rawData, double resolutionPs)
        {
            _lastResolutionPs = resolutionPs;
            _lastRawData = rawData;
            UpdatePlotRender();
        }
    }
}