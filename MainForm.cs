using SpadApp.Controller;
using SpadApp.DLLWrapper;
using SpadApp.Model;
using SpadApp.Parameters;
using SpadApp.Utility;
using SpadApp.View;
using System.Configuration;

namespace SpadApp
{
    public partial class MainForm : Form
    {

        public MainForm()
        {
            InitializeComponent();

            //MainForm.Designer에서 생성된 histogramPlot을 매니저에게 전달
            _chartManager = new HistogramChartManager(this.histogramPlot!);

            countRateMonitorTimer.Tick += PicoHarpMonitorTimer_Tick;

            powerMeterMonitorTimer1.Tick += PmMonitorTimer1_Tick;
            powerMeterMonitorTimer2.Tick += PmMonitorTimer2_Tick;
        }


        private void MainForm_Load(object sender, EventArgs e)
        {
            MeasurementParameterSetting();
        }
        private void UpdateDeviceSettings()
        {
            UpdateDeviceSettings_PicoHarp();
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            if (btnConnect.Text == "Connect")
            {
                // ------------------------------------------------------------
                // PicoHarp300 Connect
                // ------------------------------------------------------------

                bool tcspcConnected =
                    _TCSPCDeviceController.ConnectDevice(
                        countRateMonitorTimer,
                        Log);

                if (!tcspcConnected)
                {
                    Log("PicoHarp300 connection failed.");
                    return;
                }

                UpdateDeviceSettings();

                Log("PicoHarp300 connected.");

                // ------------------------------------------------------------
                // PM100USB #0 Connect
                // ------------------------------------------------------------

                bool pm1Connected =
                    _powerMeterController1.ConnectDevice(
                        0,
                        powerMeterMonitorTimer1,
                        Log);

                if (pm1Connected)
                {
                    Log("PM100USB #1 Connected");
                }
                else
                {
                    Log("PM100USB #1 connection failed.");
                }

                // ------------------------------------------------------------
                // PM100USB #1 Connect
                // ------------------------------------------------------------

                bool pm2Connected =
                    _powerMeterController2.ConnectDevice(
                        1,
                        powerMeterMonitorTimer2,
                        Log);

                if (pm2Connected)
                {
                    Log("PM100USB #2 Connected");
                }
                else
                {
                    Log("PM100USB #2 connection failed.");
                }

                // ------------------------------------------------------------
                // UI Update
                // ------------------------------------------------------------

                btnConnect.Text = "Disconnect";

                btnConnect.BackColor =
                    Color.IndianRed;

                btnMeasure.Enabled = true;
            }
            else
            {
                // ------------------------------------------------------------
                // Disconnect All Devices
                // ------------------------------------------------------------

                _TCSPCDeviceController.DisconnectDevice(Log);

                _powerMeterController1.DisconnectDevice(Log);

                _powerMeterController2.DisconnectDevice(Log);

                Log("All devices disconnected.");

                ResetUI();
            }
        }

        private async void btnMeasure_Click(object sender, EventArgs e)
        {
            if (!PicoHarp_DeviceInfo.IsConnected)
            {
                Log("Device not connected.");
                return;
            }

            // 🌟 1. 하드웨어 독점 계측을 시작하기 전, 실시간 모니터링 타이머를 중지합니다.
            countRateMonitorTimer.Stop();
            btnMeasure.Enabled = false;

            try
            {
                // 컨트롤러에게 단발성 측정 위임 (단발성이므로 checkContinue는 항상 true 반환하는 람다 전달)
                await _TCSPCDeviceController.ExecuteMeasurementAsync(_histogram.RawData, () => true);

                // 계측이 완료된 직후, 차트를 업데이트하기 전에 
                // 화면의 레이트 미터 텍스트도 최신 값으로 한 번 수동 갱신해 주면 UI가 자연스럽습니다.
                PicoHarpMonitorTimer_Tick(null, EventArgs.Empty);

                // 차트 업데이트
                _chartManager.UpdateFromHardware(_histogram.RawData);
                Log("Single measurement done.");
            }
            catch (Exception ex)
            {
                Log($"Single measurement error: {ex.Message}");
            }
            finally
            {
                // 🌟 2. [핵심] 성공/실패 여부와 상관없이 계측 태스크가 완전히 끝났으므로
                // 단발성 측정 버튼을 다시 활성화하고, 실시간 모니터링 타이머를 되살립니다.
                btnMeasure.Enabled = true;
                countRateMonitorTimer.Start();
            }
        }

        // 중복되는 차트 업데이트 로직을 별도 뺌
        private void UpdateChartAfterMeasurement()
        {

            double res = 0;
            PicoHarp_Native.PH_GetResolution(PicoHarp_DeviceInfo.DeviceIndex, ref res);
            _chartManager.Update(_histogram.RawData, res);
        }

        private bool _isRepeating = false;

        private async void btnRun_Click(object sender, EventArgs e)
        {
            if (!PicoHarp_DeviceInfo.IsConnected) return;

            if (!_isRepeating)
            {
                _isRepeating = true;
                btnRun.Text = "Stop";
                btnRun.BackColor = Color.IndianRed;

                // 🌟 1. 계측 시작 직전 진짜 타이머를 끕니다.
                countRateMonitorTimer.Stop();

                try
                {
                    while (_isRepeating)
                    {
                        await _TCSPCDeviceController.ExecuteMeasurementAsync(_histogram.RawData, () => _isRepeating);
                        if (!_isRepeating) break;

                        // 루프 중간에 레이트 미터도 한 번 갱신하고 차트를 그립니다.
                        PicoHarpMonitorTimer_Tick(null, EventArgs.Empty);
                        UpdateChartAfterMeasurement();
                    }
                }
                finally
                {
                    _isRepeating = false;
                    btnRun.Text = "Run";
                    btnRun.BackColor = SystemColors.Control;

                    // 🌟 2. 반복 계측이 완전히 정지되면 진짜 타이머를 다시 확실하게 켭니다.
                    countRateMonitorTimer.Start();
                }
            }
            else
            {
                _isRepeating = false;
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            _TCSPCDeviceController.DisconnectDevice(Log);
            ResetUI();
            _powerMeterController1.DisconnectDevice(Log);
            _powerMeterController2.DisconnectDevice(Log);
        }


        private void btnSave_Click(object sender, EventArgs e)
        {

        }

        private void MeasurementParameterSetting()
        {

            numPMWavelength.Minimum = 200;
            numPMWavelength.Maximum = 2000;
            numPMWavelength.Value = 512;

            // 안전한 파싱: AppSettings에서 값이 없거나 형식이 잘못된 경우 기본값 사용
            if (!int.TryParse(ConfigurationManager.AppSettings["AcqTime"], out int acqTime)) acqTime = 100;
            numAcqTime.Minimum = 1; numAcqTime.Maximum = 100000;
            numAcqTime.Value = (decimal)acqTime;

            // 1. SyncDiv 설정 ({1, 2, 4, 8})
            if (!int.TryParse(ConfigurationManager.AppSettings["SyncDiv"], out int syncDiv)) syncDiv = 1;
            numSyncDiv.Value = (decimal)syncDiv;
            numSyncDiv.ReadOnly = true; // 직접 타이핑 방지

            // 2. Binning 설정 (0~7)
            if (!int.TryParse(ConfigurationManager.AppSettings["Binning"], out int binning)) binning = 0;
            numBinning.Minimum = 0; numBinning.Maximum = 7;
            numBinning.Value = (decimal)binning;

            numBinning.ReadOnly = true;

            if (!int.TryParse(ConfigurationManager.AppSettings["CFDLevel0"], out int cfdLevel0)) cfdLevel0 = 50;
            numCFDLevel0.Minimum = 0; numCFDLevel0.Maximum = 800;
            numCFDLevel0.Value = (decimal)cfdLevel0;

            if (!int.TryParse(ConfigurationManager.AppSettings["CFDLevel1"], out int cfdLevel1)) cfdLevel1 = 50;
            numCFDLevel1.Minimum = 0; numCFDLevel1.Maximum = 800;
            numCFDLevel1.Value = (decimal)cfdLevel1;

            if (!int.TryParse(ConfigurationManager.AppSettings["CFDZeroCross0"], out int zx0)) zx0 = 0;
            numCFDZeroCross0.Minimum = 0; numCFDZeroCross0.Maximum = 20;
            numCFDZeroCross0.Value = (decimal)zx0;

            if (!int.TryParse(ConfigurationManager.AppSettings["CFDZeroCross1"], out int zx1)) zx1 = 0;
            numCFDZeroCross1.Minimum = 0; numCFDZeroCross1.Maximum = 20;
            numCFDZeroCross1.Value = (decimal)zx1;

            PicoHarp_MeasurementStatus.AcquisitionTimeMs = (int)numAcqTime.Value;
            PicoHarp_MeasurementSettings.SyncDivider = (int)numSyncDiv.Value;
            PicoHarp_MeasurementSettings.Binning = (int)numBinning.Value;
            PicoHarp_MeasurementSettings.CFDLevel0 = (int)numCFDLevel0.Value;
            PicoHarp_MeasurementSettings.CFDLevel1 = (int)numCFDLevel1.Value;
            PicoHarp_MeasurementSettings.CFDZeroCross0 = (int)numCFDZeroCross0.Value;
            PicoHarp_MeasurementSettings.CFDZeroCross1 = (int)numCFDZeroCross1.Value;

            btnPM1ZeroAdjust.BackColor = Color.LightGreen;
            btnPM1ZeroAdjust.Text = "Background OFF";
        }
        /// <summary>
        /// 연결이 끊겼을 때 UI 요소들을 초기 상태로 돌립니다.
        /// </summary>
        private void ResetUI()
        {
            btnConnect.Text = "Connect";
            btnConnect.BackColor = SystemColors.Control;
            btnMeasure.Enabled = false;
            lblCountRate0.Text = "Count Rate 0 : 0";
            lblCountRate1.Text = "Count Rate 1 : 0";
            lblResolution.Text = "0";
        }


    }

}