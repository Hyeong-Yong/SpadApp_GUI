using ScottPlot;
using ScottPlot.WinForms;
using SpadApp.Model;

namespace SpadApp.View
{
    public class HistogramChartManager
    {
        private readonly FormsPlot _plot;
        private ScottPlot.Plottables.Signal? _signal;
        private double[] _plotData;

        /// <summary>
        /// 생성자: MainForm의 histogramPlot 컨트롤을 받아 초기화합니다.
        /// </summary>
        /// <param name="plot">WinForm에 배치된 FormsPlot 컨트롤</param>
        public HistogramChartManager(FormsPlot plot)
        {
            _plot = plot;
            // PicoHarp 300의 표준 히스토그램 채널 수(65536)에 맞춰 배열 생성 [cite: 830, 1088]
            _plotData = new double[PicoHarpDevice.HISTCHAN];
            InitChart();
        }

        /// <summary>
        /// 차트의 초기 스타일과 레이블을 설정합니다.
        /// </summary>
        private void InitChart()
        {
            _plot.Plot.Clear();

            // 1. 시그널 그래프 생성 (데이터 소스를 _plotData 배열로 직접 연결)
            _signal = _plot.Plot.Add.Signal(_plotData);
            _signal.Color = Colors.Blue;
            _signal.LineWidth = 1.5f;

            // 2. 요청하신 차트 타이틀 및 축 레이블 설정
            _plot.Plot.Title("Time-Resolved Photon Spectrum");
            _plot.Plot.XLabel("Time (ns)");
            _plot.Plot.YLabel("Photon Counts (number)");

            // 3. 눈금 및 그리드 스타일 조정
            _plot.Plot.Grid.MajorLineColor = Colors.LightGray.WithAlpha(0.5);

            _plot.Refresh();
        }

        /// <summary>
        /// 데이터는 건드리지 않고, Binning 변경 등에 따른 X축 해상도(눈금)만 즉시 업데이트합니다.
        /// </summary>
        /// <param name="resolutionPs">장비에서 읽어온 현재 Resolution (ps 단위) [cite: 841]</param>
        public void UpdateResolution(double resolutionPs)
        {
            if (_signal == null) return;

            // ps 단위를 ns로 변환하여 시그널의 간격(Period) 업데이트
            double resNs = resolutionPs / 1000.0;
            _signal.Data.Period = resNs;

            // X축 범위만 자동으로 맞춤
            _plot.Plot.Axes.AutoScaleX();
            _plot.Refresh();
        }

        /// <summary>
        /// 측정 완료 후, 새로운 히스토그램 데이터와 해상도를 차트에 반영합니다.
        /// </summary>
        /// <param name="rawData">PicoHarp에서 읽어온 uint 배열 [cite: 827]</param>
        /// <param name="resolutionPs">현재 Resolution (ps 단위) [cite: 841]</param>
        public void Update(uint[] rawData, double resolutionPs)
        {
            if (_signal == null) return;

            // 1. X축 스케일 업데이트 (ps -> ns)
            double resNs = resolutionPs / 1000.0;
            _signal.Data.Period = resNs;

            // 2. 히스토그램 데이터 복사 (uint -> double)
            // ScottPlot 5 Signal은 연결된 배열의 값이 바뀌면 Refresh 시 바로 반영됩니다.
            for (int i = 0; i < rawData.Length; i++)
            {
                _plotData[i] = rawData[i];
            }

            // 3. 전체 축 자동 조절 및 화면 갱신
            _plot.Plot.Axes.AutoScale();
            _plot.Refresh();
        }
    }
}