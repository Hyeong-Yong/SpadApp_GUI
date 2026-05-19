using SpadApp.Controller;
using SpadApp.DLLWrapper;
using SpadApp.Parameters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace SpadApp
{
    public partial class ucPDE : UserControl
    {

        private readonly DeviceController_PicoHarp300 _TCSPCDeviceController;
        private readonly ucDCR _ucDCR; // DCR 값을 가져오기 위한 참조 변수
        // 🌟 실시간으로 업데이트되는 Incident Photon Number를 보관할 변수
        private double _currentIncidentPhotonNumber = 0;

        public ucPDE(DeviceController_PicoHarp300 tcspc, ucDCR ucDcr)
        {
            InitializeComponent();
            _TCSPCDeviceController = tcspc;
            _ucDCR = ucDcr;
        }


        // 🌟 MainForm에서 호출해주는 실시간 UI 업데이트 전용 메서드
        public void UpdateIncidentPhotonNumber(double flux)
        {
            _currentIncidentPhotonNumber = flux;

            // 요청하신 소수점 버림 규칙(:F0)을 적용하여 텍스트박스에 실시간 표출
            lbl_IPN.Text = flux.ToString("F0");
        }

        private void btnPDEmeasure_Click(object sender, EventArgs e)
        {
            MeasureAveragePDEAsync();
        }

        private async Task MeasureAveragePDEAsync()
        {
            // 1. 조건 검사: ucDCR에서 선행 측정된 DCR 값이 있는지 확인
            if (_ucDCR.LatestAverageDCR == null)
            {
                MessageBox.Show("DCR 측정이 선행되지 않았습니다.\n먼저 DCR 측정 화면에서 측정을 완료해주세요.",
                                "알림", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            double nDCR = _ucDCR.LatestAverageDCR.Value;
            int avgCount = 10;

            // 3. 하드웨어 독점 및 UI 잠금
            _TCSPCDeviceController.StopMonitoring();
            btnPDEmeasure.Enabled = false;

            try
            {
                double pdeSum = 0;

                // UI 초기화 및 N_DCR 선행값 고정 표시
                lbl_DCR.Text = nDCR.ToString("F0");
                progressBarPDE.Minimum = 0;
                progressBarPDE.Maximum = avgCount;
                progressBarPDE.Value = 0;

                // ScottPlot 셋업
                PDEplot.Plot.Clear();
                PDEplot.Plot.XLabel("Index");
                PDEplot.Plot.YLabel("Photon Detection Efficiency (%)");

                // 실시간 플롯용 고정 배열 생성 및 등록
                double[] pdeArray = new double[avgCount];
                var signalPlot = PDEplot.Plot.Add.Signal(pdeArray);
                signalPlot.Data.MaximumIndex = 0;

                // 4. PDE 실시간 루프 계측 시작
                for (int i = 0; i < avgCount; i++)
                {
                    if (!PicoHarp_DeviceInfo.IsConnected) return;

                    // 🌟 실시간으로 변하고 있는 파워메터 기반 Flux 값을 매 루프마다 가져옴
                    double nInc = _currentIncidentPhotonNumber;

                    // 파워미터 백그라운드 캘리브레이션 미수행 혹은 광원 OFF 등으로 0 이하(마이너스 포함)의 값이 들어오면 중단 처리
                    if (nInc <= 0)
                    {
                        MessageBox.Show("Incident Photon Number(입사 광자 수)가 0 이하입니다.\n\n" +
                                        "원인 분석:\n" +
                                        "1. 파워미터의 Background Calibration(Zero Adjust)이 수행되지 않았을 수 있습니다.\n" +
                                        "2. 현재 레이저/광원이 완전히 꺼져(OFF) 있을 수 있습니다.\n\n" +
                                        "확인 후 다시 시도해 주세요. 측정을 중단합니다.",
                                        "측정 오류 및 중단", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return; 
                    }

                    int countRate1 = 0;
                    int ret = PicoHarp_Native.PH_GetCountRate(PicoHarp_DeviceInfo.DeviceIndex, 1, ref countRate1);
                    PicoHarp_MeasurementStatus.CountRate1 = countRate1;

                    if (ret != 0)
                    {
                        MessageBox.Show($"PH_GetCountRate Error: {ret}");
                        return;
                    }

                    // 수식 계산: PDE = (N_dpn - N_DCR) / N_inc * 100
                    double currentPDE = ((countRate1 - nDCR) / nInc) * 100;

                    // 누적 및 평균 계산
                    pdeSum += currentPDE;
                    double averagePDE = pdeSum / (i + 1);

                    // 데이터 배열 업데이트 및 차트 렌더링
                    pdeArray[i] = currentPDE;
                    signalPlot.Data.MaximumIndex = i;
                    PDEplot.Plot.Axes.AutoScale();
                    PDEplot.Refresh();

                    // 실시간 UI 텍스트 업데이트 (소수점 버림 포맷 :F0 적용)
                    lbl_DPN.Text = countRate1.ToString("F0");
                    lbl_PDE.Text = averagePDE.ToString("F0");
                    progressBarPDE.Value = i + 1;

                    // 120ms 비동기 대기 (UI Freeze 방지 및 드라이버 안정화)
                    await Task.Delay(120);
                }

                double finalAveragePDE = pdeSum / avgCount;
                MessageBox.Show($"Final Average PDE = {finalAveragePDE:F0} %");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"PDE 측정 중 오류 발생: {ex.Message}");
            }
            finally
            {
                // 5. 자원 원상 복구
                btnPDEmeasure.Enabled = true;
                _TCSPCDeviceController.StartMonitoring();
            }
        }


        private void ucPDE_Load(object sender, EventArgs e)
        {

        }

    }
}
