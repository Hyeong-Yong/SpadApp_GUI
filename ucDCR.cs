using ScottPlot;
using SpadApp.Controller;
using SpadApp.DLLWrapper;
using SpadApp.Parameters;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpadApp
{
    public partial class ucDCR : UserControl
    {
        private readonly List<double> dcrData = new();
        private readonly DeviceController_PicoHarp300 _TCSPCDeviceController;
        public double? LatestAverageDCR { get; private set; } = null;

        // 생성자: 컨트롤러 내부에 타이머 제어 기능이 있으므로 Timer 주입 단자를 제거하여 결합도를 낮춥니다.
        public ucDCR(DeviceController_PicoHarp300 tcspc)
        {
            InitializeComponent();
            _TCSPCDeviceController = tcspc;
        }
        private async Task MeasureAverageDCRAsync()
        {
            int avgCount = (int)numAverageCount.Value;
            if (avgCount <= 0) return;

            // 1. 하드웨어 독점 및 버튼 비활성화
            _TCSPCDeviceController.StopMonitoring();
            btnMeasure.Enabled = false;

            try
            {
                double sum = 0;

                progressBar1.Minimum = 0;
                progressBar1.Maximum = avgCount;
                progressBar1.Value = 0;

                // 🌟 [개선 1] 그래프 초기화 및 X, Y축 라벨 설정을 루프 외부에서 딱 한 번만 수행
                DcrPlot.Plot.Clear();
                DcrPlot.Plot.XLabel("Index");
                DcrPlot.Plot.YLabel("Dark Current Rate");

                // 🌟 [개선 2] 매번 배열을 새로 만들지 않도록 전체 크기(avgCount)의 고정 배열을 미리 생성
                double[] dcrArray = new double[avgCount];

                // ScottPlot에 데이터 소스를 한 번만 등록 (X축은 자동으로 0, 1, 2... 인덱스 매핑)
                var signalPlot = DcrPlot.Plot.Add.Signal(dcrArray);
                signalPlot.Data.MaximumIndex = 0; // 처음엔 아무것도 그리지 않거나 첫 포인트만 세팅

                for (int i = 0; i < avgCount; i++)
                {
                    if (!PicoHarp_DeviceInfo.IsConnected) return;

                    int countRate1 = 0;

                    // 채널 1 (Photon) 레이트 취득 
                    int ret = PicoHarp_Native.PH_GetCountRate(PicoHarp_DeviceInfo.DeviceIndex, 1, ref countRate1);
                    PicoHarp_MeasurementStatus.CountRate1 = countRate1;

                    if (ret != 0)
                    {
                        MessageBox.Show($"PH_GetCountRate Error: {ret}");
                        return;
                    }

                    // 누적 및 평균 계산
                    sum += countRate1;
                    double averageDCR = sum / (i + 1);

                    // 🌟 [개선 3] 실시간 데이터 배열의 현재 인덱스(i)에 값만 업데이트
                    dcrArray[i] = countRate1;

                    // 아직 측정되지 않은 뒷부분(0으로 채워진 영역)은 그래프에 그려지지 않도록 제한
                    signalPlot.Data.MaximumIndex = i;

                    // 🌟 [개선 4] 데이터 추가에 맞춰 축 범위를 자동 조절하고 깜빡임 없이 새로고침
                    DcrPlot.Plot.Axes.AutoScale();
                    DcrPlot.Refresh();

                    // UI 텍스트 및 프로그래스바 업데이트
                    lblAverageDCR.Text = $"Average DCR : {averageDCR:F0} cps";
                    progressBar1.Value = i + 1;

                    // 10ms 비동기 대기
                    await Task.Delay(10);
                }

                double finalAverage = sum / avgCount;
                LatestAverageDCR = finalAverage;

                MessageBox.Show($"Final Average DCR = {finalAverage:F0} cps");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"DCR 측정 중 예외 발생: {ex.Message}");
            }
            finally
            {
                // 하드웨어 타이머 복구
                btnMeasure.Enabled = true;
                _TCSPCDeviceController.StartMonitoring();
            }
        }

        // async void 이벤트 핸들러 내부에서 Task 메서드를 안정적으로 await 합니다.
        private async void btnMeasure_Click(object sender, EventArgs e)
        {
            await MeasureAverageDCRAsync();
        }

        private void ucDCR_Load(object sender, EventArgs e)
        {
            numAverageCount.Value = 10; // 기본값 설정
        }
    }
}