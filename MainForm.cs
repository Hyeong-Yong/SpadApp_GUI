using SpadApp.Controller;
using SpadApp.Model;
using SpadApp.Parameters;
using SpadApp.View;
using System.Configuration;
using System.Windows.Forms;

namespace SpadApp
{
    public partial class MainForm : Form
    {

        private HistogramBuffer _histogram = new();
        private HistogramChartManager _chartManager;

        public MainForm()
        {
            InitializeComponent();
            //MainForm.Designer에서 생성된 histogramPlot을 매니저에게 전달
            _chartManager = new HistogramChartManager(this.histogramPlot);
            countRateMonitorTimer.Tick += MonitorTimer_Tick;
        }

        private System.Windows.Forms.Timer countRateMonitorTimer = new();
        private DeviceController _deviceController = new();

        private void MainForm_Load(object sender, EventArgs e)
        {
            MeasurementParameterSetting();
        }
        private void UpdateDeviceSettings()
        {
            if (!PicoHarpInfo.IsConnected) return;

            // 1. UI에서 현재 설정값 읽어오기
            MeasurementSettings.AcqTime = (int)numAcqTime.Value;
            MeasurementSettings.SyncDivider = (int)numSyncDiv.Value;
            MeasurementSettings.Binning = (int)numBinning.Value;
            MeasurementSettings.CFDLevel0 = (int)numCFDLevel0.Value;
            MeasurementSettings.CFDZeroCross0 = (int)numCFDZeroCross0.Value;
            MeasurementSettings.CFDLevel1 = (int)numCFDLevel1.Value;
            MeasurementSettings.CFDZeroCross1 = (int)numCFDZeroCross1.Value;

            // 2. 장비에 설정 적용
            PicoHarpDevice.PH_SetSyncDiv(PicoHarpInfo.DeviceIndex, MeasurementSettings.SyncDivider);
            PicoHarpDevice.PH_SetInputCFD(PicoHarpInfo.DeviceIndex, 0, MeasurementSettings.CFDLevel0, MeasurementSettings.CFDZeroCross0);
            PicoHarpDevice.PH_SetInputCFD(PicoHarpInfo.DeviceIndex, 1, MeasurementSettings.CFDLevel1, MeasurementSettings.CFDZeroCross1);
            PicoHarpDevice.PH_SetBinning(PicoHarpInfo.DeviceIndex, MeasurementSettings.Binning);

            // 3. 설정 변경에 따른 새로운 Resolution 값 취득 및 UI 업데이트
            double resolution = 0;
            PicoHarpDevice.PH_GetResolution(PicoHarpInfo.DeviceIndex, ref resolution);
            lblResolution.Text = resolution.ToString();

            // 차트 매니저에게도 변경된 해상도를 알림 (X축 스케일 갱신을 위해 필요 시 호출)
            _chartManager.UpdateTimeAxis(resolution);
        }



        private void Log(string msg)
        {
            richtxtLog.AppendText(msg + Environment.NewLine);
        }


        private void btnConnect_Click(object sender, EventArgs e)
        {
            if (btnConnect.Text == "Connect")
            {
                // 장치 연결 및 타이머 시작 시도
                if (_deviceController.ConnectDevice(countRateMonitorTimer, Log))
                {
                    UpdateDeviceSettings(); // 초기 설정값 장비에 주입

                    btnConnect.Text = "Disconnect";
                    btnConnect.BackColor = Color.IndianRed;
                    btnMeasure.Enabled = true;
                }
            }
            else
            {
                // 하드웨어 해제 로직 호출
                _deviceController.DisconnectDevice(Log);
                // UI 컨트롤 초기화
                ResetUI();
            }
        }


        private void MonitorTimer_Tick(object? sender, EventArgs e)
        {
            if (!PicoHarpInfo.IsConnected) return;

            int rate0 = 0;
            int rate1 = 0;

            // 채널 0 (Sync) 레이트 취득 
            int ret0 = PicoHarpDevice.PH_GetCountRate(PicoHarpInfo.DeviceIndex, 0, ref rate0);
            // 채널 1 (Photon) 레이트 취득 
            int ret1 = PicoHarpDevice.PH_GetCountRate(PicoHarpInfo.DeviceIndex, 1, ref rate1);

            // UI 업데이트 (ToolStripStatusLabel)
            if (ret0 >= 0)
                lblCountRate0.Text = $"Count Rate 0 : {rate0}";

            if (ret1 >= 0)
                lblCountRate1.Text = $"Count Rate 1 : {rate1}";
        }

        private async void btnMeasure_Click(object sender, EventArgs e)
        {
            btnMeasure.Enabled = false;

            // 컨트롤러에게 측정을 시킴 (checkContinue는 항상 true 반환)
            await _deviceController.ExecuteMeasurementAsync(_histogram.RawData, () => true);

            // 차트 업데이트
            _chartManager.UpdateFromHardware(_histogram.RawData);
            Log("Single measurement done.");
            btnMeasure.Enabled = true;
        }

        // 중복되는 차트 업데이트 로직을 별도로 뺐습니다.
        private void UpdateChartAfterMeasurement()
        {
            double res = 0;
            PicoHarpDevice.PH_GetResolution(PicoHarpInfo.DeviceIndex, ref res);
            _chartManager.Update(_histogram.RawData, res);
        }

        private bool _isRepeating = false;

        private async void btnRun_Click(object sender, EventArgs e)
        {
            if (!_isRepeating){
                _isRepeating = true;
                btnRun.Text = "Stop";
                btnRun.BackColor = Color.IndianRed;
                Log("Continuous measurement started.");
                try{
                    while (_isRepeating)
                    {
                        // 컨트롤러에게 측정 위임 (현재 _isRepeating 상태를 델리게이트로 전달)
                        await _deviceController.ExecuteMeasurementAsync(_histogram.RawData, () => _isRepeating);
                        if (!_isRepeating) break;
                        UpdateChartAfterMeasurement();
                    }
                }
                finally{
                    _isRepeating = false;
                    btnRun.Text = "Run";
                    btnRun.BackColor = SystemColors.Control;
                }
            }
            else{
                _isRepeating = false;
            }
        }


        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            _deviceController.DisconnectDevice(Log);
            ResetUI();
        }


        private void btnSave_Click(object sender, EventArgs e)
        {

        }

        private decimal lastSyncValue = 1;
        private void numSyncDiv_ValueChanged(object sender, EventArgs e)
        {
            int current = (int)numSyncDiv.Value;

            if (current > lastSyncValue){ // 위로 버튼(▲)을 눌렀을 때\
                if (lastSyncValue == 1) numSyncDiv.Value = 2; else if (lastSyncValue == 2) numSyncDiv.Value = 4; else if (lastSyncValue == 4) numSyncDiv.Value = 8;else if (lastSyncValue == 8) numSyncDiv.Value = 8; // 8이면 고정
            }else if (current < lastSyncValue){ // 아래로 버튼(▼)을 눌렀을 때
                if (lastSyncValue == 8) numSyncDiv.Value = 4;else if (lastSyncValue == 4) numSyncDiv.Value = 2; else if (lastSyncValue == 2) numSyncDiv.Value = 1;else if (lastSyncValue == 1) numSyncDiv.Value = 1; // 1이면 고정
            }

            // 현재 값을 다시 저장
            lastSyncValue = numSyncDiv.Value;

            // 장비 설정값에 반영
            MeasurementSettings.SyncDivider = (int)numSyncDiv.Value;
            UpdateDeviceSettings();
        }

        private void numBinning_ValueChanged(object sender, EventArgs e) => UpdateDeviceSettings();
        private void numAcqTime_ValueChanged(object sender, EventArgs e) => UpdateDeviceSettings();
        private void numCFDLevel0_ValueChanged(object sender, EventArgs e) => UpdateDeviceSettings();
        private void numCFDZeroCross0_ValueChanged(object sender, EventArgs e) => UpdateDeviceSettings();
        private void numCFDLevel1_ValueChanged(object sender, EventArgs e) => UpdateDeviceSettings();
        private void numCFDZeroCross1_ValueChanged(object sender, EventArgs e) => UpdateDeviceSettings();

        private void MeasurementParameterSetting()
        {

            // 안전한 파싱: AppSettings에서 값이 없거나 형식이 잘못된 경우 기본값 사용
            if (!int.TryParse(ConfigurationManager.AppSettings["AcqTime"], out int acqTime)) acqTime = 100;
            numAcqTime.Minimum = 1;            numAcqTime.Maximum = 100000;
            numAcqTime.Value = (decimal)acqTime;

            // 1. SyncDiv 설정 ({1, 2, 4, 8})
            if (!int.TryParse(ConfigurationManager.AppSettings["SyncDiv"], out int syncDiv)) syncDiv = 1;
            numSyncDiv.Value = (decimal)syncDiv;
            numSyncDiv.ReadOnly = true; // 직접 타이핑 방지

            // 2. Binning 설정 (0~7)
            if (!int.TryParse(ConfigurationManager.AppSettings["Binning"], out int binning)) binning = 0;
            numBinning.Minimum = 0;            numBinning.Maximum = 7;
            numBinning.Value = (decimal)binning;

            numBinning.ReadOnly = true;

            if (!int.TryParse(ConfigurationManager.AppSettings["CFDLevel0"], out int cfdLevel0)) cfdLevel0 = 50;
            numCFDLevel0.Minimum = 0;            numCFDLevel0.Maximum = 800;
            numCFDLevel0.Value = (decimal)cfdLevel0;

            if (!int.TryParse(ConfigurationManager.AppSettings["CFDLevel1"], out int cfdLevel1)) cfdLevel1 = 50;
            numCFDLevel1.Minimum = 0;            numCFDLevel1.Maximum = 800;
            numCFDLevel1.Value = (decimal)cfdLevel1;

            if (!int.TryParse(ConfigurationManager.AppSettings["CFDZeroCross0"], out int zx0)) zx0 = 0;
            numCFDZeroCross0.Minimum = 0;            numCFDZeroCross0.Maximum = 20;
            numCFDZeroCross0.Value = (decimal)zx0;

            if (!int.TryParse(ConfigurationManager.AppSettings["CFDZeroCross1"], out int zx1)) zx1 = 0;
            numCFDZeroCross1.Minimum = 0;            numCFDZeroCross1.Maximum = 20;
            numCFDZeroCross1.Value = (decimal)zx1;

            MeasurementSettings.AcqTime = (int)numAcqTime.Value;
            MeasurementSettings.SyncDivider = (int)numSyncDiv.Value;
            MeasurementSettings.Binning = (int)numBinning.Value;
            MeasurementSettings.CFDLevel0 = (int)numCFDLevel0.Value;
            MeasurementSettings.CFDLevel1 = (int)numCFDLevel1.Value;
            MeasurementSettings.CFDZeroCross0 = (int)numCFDZeroCross0.Value;
            MeasurementSettings.CFDZeroCross1 = (int)numCFDZeroCross1.Value;
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