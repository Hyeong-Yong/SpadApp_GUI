using SpadApp.Controller;
using SpadApp.Model;
using SpadApp.Parameters;
using SpadApp.View;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace SpadApp
{
    public partial class ucJitter : UserControl
    {
        public DeviceController_PicoHarp300 TCSPCDeviceController = new();
        private HistogramBuffer _histogram = new();
        private HistogramChartManager _chartManager;

        public ucJitter(DeviceController_PicoHarp300 tcspc)
        {
            InitializeComponent();
            TCSPCDeviceController = tcspc;

            _chartManager = new HistogramChartManager(this.JitterPlot!);
        }

        private void ucJitter_Load(object sender, EventArgs e)
        {
            numLaserPulseWidth.Minimum = 10;
            numLaserPulseWidth.Maximum = 300;

            numLaserPulseWidth.Value = 30;
            numTcspcJitter.Value = 12;

            lblSystemJitter.Text = "System Jitter (FWHM): - ps";
            lblSpadJitter.Text = "SPAD Jitter: - ps";
        }

        private async void btnJitterMeasure_Click(object sender, EventArgs e)
        {
            if (!PicoHarp_DeviceInfo.IsConnected)
            {
                MessageBox.Show("Device not connected.");
                return;
            }

            TCSPCDeviceController.StopMonitoring();
            btnJitterMeasure.Enabled = false;

            try
            {
                await TCSPCDeviceController.ExecuteMeasurementAsync(_histogram.RawData, () => true);

                // 3. (요청 반영) 지터 분석 및 차트 그리기 메서드 호출
                CalculateJitter(_histogram.RawData);

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Jitter Measurement error: {ex.Message}");
            }
            finally
            {
                btnJitterMeasure.Enabled = true;
                TCSPCDeviceController.StartMonitoring();
            }
        }

        // 4. (요청 반영) 메서드 이름 변경 및 JitterPlot 차트 업데이트 로직 포함
        private void CalculateJitter(uint[] rawData)
        {
            double[] histogramDouble = rawData.Select(x => (double)x).ToArray();
            double binWidthPs = PicoHarp_MeasurementStatus.ResolutionPs;

            // X축 배열 생성 (ps 단위)
            double[] xPositions = Enumerable.Range(0, histogramDouble.Length).Select(i => i * binWidthPs).ToArray();

            // 계산기 메서드 호출 (이름 변경됨)
            var fitResult = GaussianCalculator.GaussianCalculateBin(histogramDouble, binWidthPs);

            // ==============================================================
            // ★ ScottPlot (JitterPlot) 차트 렌더링 로직
            // ==============================================================
            JitterPlot.Plot.Clear(); // 기존 차트 지우기

            // ① 원본 Raw 데이터 그리기 (파란색 점)
            var rawScatter = JitterPlot.Plot.Add.Scatter(xPositions, histogramDouble);
            rawScatter.LineWidth = 0;       // 선 숨김 (점만 표시)
            rawScatter.MarkerSize = 4;
            rawScatter.Color = ScottPlot.Colors.LightBlue;

            if (fitResult.IsSuccess)
            {
                // ② 가우시안 피팅 결과 선 그리기 (빨간색 선)
                int fitPoints = 1000;
                double[] fitX = new double[fitPoints];
                double[] fitY = new double[fitPoints];

                // 피크 주변(Mean ± 4 Sigma) 구간만 부드럽게 그림
                double xMin = fitResult.Mean - (4 * fitResult.Sigma);
                double xMax = fitResult.Mean + (4 * fitResult.Sigma);
                double step = (xMax - xMin) / (fitPoints - 1);

                for (int i = 0; i < fitPoints; i++)
                {
                    double x = xMin + (i * step);
                    fitX[i] = x;
                    // 가우시안 수학 공식 적용
                    fitY[i] = fitResult.Amplitude * Math.Exp(-Math.Pow(x - fitResult.Mean, 2) / (2 * Math.Pow(fitResult.Sigma, 2)));
                }

                var fitLine = JitterPlot.Plot.Add.Scatter(fitX, fitY);
                fitLine.MarkerSize = 0;     // 점 숨김 (선만 표시)
                fitLine.LineWidth = 2.5f;
                fitLine.Color = ScottPlot.Colors.Red; // 빨간색 선 지정

                // ③ 지터 결과 UI 출력
                double t_sys = fitResult.Fwhm;
                lblSystemJitter.Text = $"System Jitter (FWHM): {t_sys:F1} ps";

                double t_laser = (double)numLaserPulseWidth.Value;
                double t_tcspc = (double)numTcspcJitter.Value;
                double squaredSpad = Math.Pow(t_sys, 2) - Math.Pow(t_laser, 2) - Math.Pow(t_tcspc, 2);

                if (squaredSpad > 0)
                {
                    double t_spad = Math.Sqrt(squaredSpad);
                    lblSpadJitter.Text = $"SPAD Jitter: {t_spad:F1} ps";
                    lblSpadJitter.ForeColor = Color.Green;
                }
                else
                {
                    lblSpadJitter.Text = "SPAD Jitter: 계산 불가 (시스템 지터가 스펙보다 작음)";
                    lblSpadJitter.ForeColor = Color.Orange;
                }

                // 차트 화면을 피크 중심으로 확대 (선택 사항)
                JitterPlot.Plot.Axes.SetLimitsX(fitResult.Mean - (6 * fitResult.Sigma), fitResult.Mean + (6 * fitResult.Sigma));
            }
            else
            {
                lblSystemJitter.Text = "System Jitter: 피팅 실패";
                lblSpadJitter.Text = $"에러: {fitResult.ErrorMessage}";
                lblSpadJitter.ForeColor = Color.Red;
                JitterPlot.Plot.Axes.AutoScale(); // 피팅 실패시 전체 데이터 보여주기
            }

            // 차트 꾸미기 및 새로고침
            JitterPlot.Plot.Title("System Jitter & Gaussian Fit");
            JitterPlot.Plot.XLabel("Time (ps)");
            JitterPlot.Plot.YLabel("Counts");
            JitterPlot.Plot.ShowLegend();
            JitterPlot.Refresh();
        }

    }
}
