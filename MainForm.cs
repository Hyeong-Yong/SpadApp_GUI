using SpadApp.Model;
using SpadApp.View;
using System.Configuration;


namespace SpadApp
{
    public partial class MainForm : Form
    {

        private HistogramBuffer _histogram = new();
        private HistogramChartManager _chartManager;

        public MainForm()
        {
            InitializeComponent();
            //디자이너에서 생성된 histogramPlot을 매니저에게 전달
            _chartManager = new HistogramChartManager(this.histogramPlot);
        }


        private System.Windows.Forms.Timer countRateMonitorTimer;


        private void MainForm_Load(object sender, EventArgs e)
        {
            numAcqTime.Value = int.Parse(ConfigurationManager.AppSettings["AcqTime"]);
            numAcqTime.Minimum = 1;
            numAcqTime.Maximum = 100000;

            // 1. SyncDiv 설정 ({1, 2, 4, 8})
            numSyncDiv.Value = int.Parse(ConfigurationManager.AppSettings["SyncDiv"]);
            numSyncDiv.ReadOnly = true; // 직접 타이핑 방지

            // 2. Binning 설정 (0~7)
            numBinning.Value = int.Parse(ConfigurationManager.AppSettings["Binning"]);
            numBinning.Minimum = 0;
            numBinning.Maximum = 7;
            numBinning.ReadOnly = true;
            numCFDLevel0.Value = int.Parse(ConfigurationManager.AppSettings["CFDLevel0"]);
            numCFDLevel0.Minimum = 0;
            numCFDLevel0.Maximum = 800;
            numCFDZeroCross0.Value = int.Parse(ConfigurationManager.AppSettings["CFDZeroCross0"]);
            numCFDZeroCross0.Minimum = 0;
            numCFDZeroCross0.Maximum = 20;
            numCFDLevel1.Value = int.Parse(ConfigurationManager.AppSettings["CFDLevel1"]);
            numCFDLevel1.Minimum = 0;
            numCFDLevel1.Maximum = 0;
            numCFDZeroCross1.Value = int.Parse(ConfigurationManager.AppSettings["CFDZeroCross1"]);
            numCFDZeroCross1.Minimum = 0;
            numCFDZeroCross1.Maximum = 20;

            MeasurementSettings.AcqTime = (int)numAcqTime.Value;
            MeasurementSettings.SyncDivider = (int)numSyncDiv.Value;
            MeasurementSettings.Binning = (int)numBinning.Value;
            MeasurementSettings.CFDLevel0 = (int)numCFDLevel0.Value;
            MeasurementSettings.CFDZeroCross0 = (int)numCFDZeroCross0.Value;
            MeasurementSettings.CFDLevel1 = (int)numCFDLevel1.Value;
            MeasurementSettings.CFDLevel1 = (int)numCFDZeroCross1.Value;
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
            _chartManager.UpdateResolution(resolution); 
        }



        private void Log(string msg)
        {
            richtxtLog.AppendText(msg + Environment.NewLine);
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {

            int ret;

            // =========================================================
            // Library Version Check
            // =========================================================
            ret = PicoHarpDevice.PH_GetLibraryVersion(PicoHarpInfo.LibVer);
            if (ret < 0)
            {
                PicoHarpDevice.PH_GetErrorString(PicoHarpInfo.ErrStr, ret);
                Log($"PH_GetLibraryVersion Error: {PicoHarpInfo.ErrStr}");
                return;
            }
            Log($"PHLib Version = {PicoHarpInfo.LibVer}");
            if (PicoHarpInfo.LibVer.ToString() != PicoHarpDevice.TargetLibVersion)
            {
                Log($"Required PHLib Version = {PicoHarpDevice.TargetLibVersion}");
                return;
            }

            // =====================================================
            // Search Devices
            // =====================================================
            Log("Searching for PicoHarp devices...");
            Log("Devidx     Status");
            PicoHarpInfo.AvailableDevices.Clear();

            for (int i = 0; i < PicoHarpDevice.MAXDEVNUM; i++)
            {
                PicoHarpInfo.Serial.Clear();
                ret = PicoHarpDevice.PH_OpenDevice(i, PicoHarpInfo.Serial);

                if (ret == 0)
                {
                    PicoHarpInfo.AvailableDevices.Add(i);
                    Log($"  {i}        S/N {PicoHarpInfo.Serial}");
                }
                else
                {
                    if (ret == PicoHarpDevice.PH_ERROR_DEVICE_OPEN_FAIL)
                    {
                        Log($"  {i}        no device");
                    }
                    else
                    {
                        PicoHarpDevice.PH_GetErrorString(PicoHarpInfo.ErrStr, ret);
                        Log($"  {i}        {PicoHarpInfo.ErrStr}");
                    }
                }
            }

            // =====================================================
            // Use First Device
            // =====================================================
            PicoHarpInfo.DeviceIndex = PicoHarpInfo.AvailableDevices[0];
            PicoHarpInfo.IsConnected = true;
            Log($"Using device {PicoHarpInfo.DeviceIndex}");

            // =====================================================
            // Initialize 진행
            // =====================================================
            Log($"Connected: {PicoHarpInfo.Serial}");

            PicoHarpDevice.PH_Initialize(PicoHarpInfo.DeviceIndex, PicoHarpDevice.MODE_HIST);
            PicoHarpDevice.PH_Calibrate(PicoHarpInfo.DeviceIndex);
            PicoHarpDevice.PH_SetSyncDiv(PicoHarpInfo.DeviceIndex, MeasurementSettings.SyncDivider);
            PicoHarpDevice.PH_SetInputCFD(PicoHarpInfo.DeviceIndex, 0, MeasurementSettings.CFDLevel0, MeasurementSettings.CFDZeroCross0);
            PicoHarpDevice.PH_SetInputCFD(PicoHarpInfo.DeviceIndex, 1, MeasurementSettings.CFDLevel1, MeasurementSettings.CFDZeroCross1);
            PicoHarpDevice.PH_SetBinning(PicoHarpInfo.DeviceIndex, MeasurementSettings.Binning);
            PicoHarpDevice.PH_SetOffset(PicoHarpInfo.DeviceIndex, MeasurementSettings.Offset);
            double resolution = 0;
            PicoHarpDevice.PH_GetResolution(PicoHarpInfo.DeviceIndex, ref resolution);
            lblResolution.Text = resolution.ToString();


            countRateMonitorTimer = new System.Windows.Forms.Timer();
            countRateMonitorTimer.Interval = 100; // 100ms 간격 [cite: 863]
            countRateMonitorTimer.Tick += MonitorTimer_Tick;

            // 연결 성공 후 타이머 시작
            countRateMonitorTimer.Start();

        }

        private void MonitorTimer_Tick(object sender, EventArgs e)
        {
            if (!PicoHarpInfo.IsConnected) return;

            int rate0 = 0;
            int rate1 = 0;

            // 채널 0 (Sync) 레이트 취득 [cite: 852]
            int ret0 = PicoHarpDevice.PH_GetCountRate(PicoHarpInfo.DeviceIndex, 0, ref rate0);
            // 채널 1 (Photon) 레이트 취득 [cite: 852]
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

            await Task.Run(() =>
            {
                PicoHarpDevice.PH_ClearHistMem(PicoHarpInfo.DeviceIndex, 0);
                PicoHarpDevice.PH_StartMeas(PicoHarpInfo.DeviceIndex, MeasurementStatus.AcquisitionTimeMs);

                int status = 0;
                while (status == 0)
                {
                    PicoHarpDevice.PH_CTCStatus(PicoHarpInfo.DeviceIndex, ref status);
                    System.Threading.Thread.Sleep(10);
                }
                PicoHarpDevice.PH_StopMeas(PicoHarpInfo.DeviceIndex);
                PicoHarpDevice.PH_GetHistogram(PicoHarpInfo.DeviceIndex, _histogram.RawData, 0);
            });

            double integral = 0;
            foreach (var v in _histogram.RawData)
                integral += v;

            Log($"Measurement Done");
            Log($"Integral Count = {integral}");
            double res = 0;
            PicoHarpDevice.PH_GetResolution(PicoHarpInfo.DeviceIndex, ref res); // [cite: 841]
            _chartManager.Update(_histogram.RawData, res);

            btnMeasure.Enabled = true;
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            PicoHarpDevice.PH_CloseDevice(PicoHarpInfo.DeviceIndex);
            countRateMonitorTimer?.Stop();
        }

        private void btnDisconnect_Click(object sender, EventArgs e)
        {

        }

        private void btnStop_Click(object sender, EventArgs e)
        {

        }

        private void btnSave_Click(object sender, EventArgs e)
        {

        }

        private decimal lastSyncValue = 1;
        private void numSyncDiv_ValueChanged(object sender, EventArgs e)
        {
            int current = (int)numSyncDiv.Value;

            if (current > lastSyncValue) // 위로 버튼(▲)을 눌렀을 때
            {
                if (lastSyncValue == 1) numSyncDiv.Value = 2;
                else if (lastSyncValue == 2) numSyncDiv.Value = 4;
                else if (lastSyncValue == 4) numSyncDiv.Value = 8;
                else if (lastSyncValue == 8) numSyncDiv.Value = 8; // 8이면 고정
            }
            else if (current < lastSyncValue) // 아래로 버튼(▼)을 눌렀을 때
            {
                if (lastSyncValue == 8) numSyncDiv.Value = 4;
                else if (lastSyncValue == 4) numSyncDiv.Value = 2;
                else if (lastSyncValue == 2) numSyncDiv.Value = 1;
                else if (lastSyncValue == 1) numSyncDiv.Value = 1; // 1이면 고정
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
    }
}