using SpadApp.DLLWrapper;
using SpadApp.Parameters;
using SpadApp.View;
using System.Configuration;

namespace SpadApp
{
    public partial class ucMainView : UserControl
    {

        public ucMainView()
        {
            InitializeComponent();

            //MainForm.Designer에서 생성된 histogramPlot을 매니저에게 전달
            _chartManager = new HistogramChartManager(this.histogramPlot!);

            TCSPCDeviceController.CountRateUpdated += OnCountRateUpdated;

            powerMeterController1.PowerUpdated += OnPowerMeter1Updated;
            powerMeterController2.PowerUpdated += OnPowerMeter2Updated;
        }

        private void ucMainView_Load(object sender, EventArgs e)
        {
            MeasurementParameterSetting();

        }


        private void btnConnect_Click(object sender, EventArgs e)
        {
            if (btnConnect.Text == "Connect")
            {
                // ------------------------------------------------------------
                // PicoHarp300 Connect
                // ------------------------------------------------------------

                bool tcspcConnected =
                    TCSPCDeviceController.ConnectDevice(Log);

                if (!tcspcConnected)
                {
                    Log("PicoHarp300 connection failed.");
                    return;
                }

                DeviceSettingInit();

                Log("PicoHarp300 connected.");

                // ------------------------------------------------------------
                // PM100USB #0 Connect
                // ------------------------------------------------------------

                bool pm1Connected =
                    powerMeterController1.ConnectDevice(
                        0,
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
                    powerMeterController2.ConnectDevice(
                        1,
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

                TCSPCDeviceController.DisconnectDevice(Log);

                powerMeterController1.DisconnectDevice(Log);

                powerMeterController2.DisconnectDevice(Log);

                Log("All devices disconnected.");

                ResetUI();
            }
        }

        private void DeviceSettingInit()
        {

            UpdateDeviceSettings_PicoHarp();

            // 장비 세팅(또는 Binning) 변경 직후, 결정된 최신 해상도를 읽어와 X축 박스를 동적으로 초기화합니다.
            if (PicoHarp_DeviceInfo.IsConnected)
            {
                double resPs = PicoHarp_MeasurementStatus.ResolutionPs;

                // PicoHarp 300의 하드웨어 한계 시간 범위(ns) 계산 (65536 ch * resolution ps / 1000)
                double maxTimeNs = (PicoHarp_Native.HISTCHAN * resPs) / 1000.0;

                // UI 컨트롤에 값을 강제로 주입할 때 ValueChanged 이벤트가 중복 트리거되어 
                // 차트가 불필요하게 여러 번 리렌더링되는 현상을 막기 위해 잠시 이벤트를 뗍니다.
                numPlotMinX.ValueChanged -= numPlotMinX_ValueChanged;
                numPlotMaxX.ValueChanged -= numPlotMaxX_ValueChanged;

                // 초기 기준을 데이터 전체 범위로 강제 동기화
                numPlotMinX.Value = 0;
                numPlotMaxX.Value = (decimal)maxTimeNs; // 🌟 하드웨어에서 들어오는 진짜 범위를 초기 기준으로 설정!

                // 값 변경이 완료되었으므로 이벤트를 다시 연결합니다.
                numPlotMinX.ValueChanged += numPlotMinX_ValueChanged;
                numPlotMaxX.ValueChanged += numPlotMaxX_ValueChanged;

                // 차트 매니저에게도 변경된 해상도에 맞춰 X축 스케일을 갱신하도록 지시
                _chartManager.UpdateTimeAxis(resPs);
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
            TCSPCDeviceController.StopMonitoring();
            btnMeasure.Enabled = false;

            try
            {
                // 컨트롤러에게 단발성 측정 위임 (단발성이므로 checkContinue는 항상 true 반환하는 람다 전달)
                await TCSPCDeviceController.ExecuteMeasurementAsync(_histogram.RawData, () => true);

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
                TCSPCDeviceController.StartMonitoring();
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
                TCSPCDeviceController.StopMonitoring();

                try
                {
                    while (_isRepeating)
                    {
                        await TCSPCDeviceController.ExecuteMeasurementAsync(_histogram.RawData, () => _isRepeating);
                        if (!_isRepeating) break;

                        // 루프 중간에 레이트 미터도 한 번 갱신하고 차트를 그립니다.
                        UpdateChartAfterMeasurement();
                    }
                }
                finally
                {
                    _isRepeating = false;
                    btnRun.Text = "Run";
                    btnRun.BackColor = SystemColors.Control;

                    // 🌟 2. 반복 계측이 완전히 정지되면 진짜 타이머를 다시 확실하게 켭니다.
                    TCSPCDeviceController.StartMonitoring();
                }
            }
            else
            {
                _isRepeating = false;
            }
        }


        private void btnSave_Click(object sender, EventArgs e)
        {

        }

        private void MeasurementParameterSetting()
        {
            // ★ X축 가로 제어 NumericUpDown 컨트롤 규격 세팅
            numPlotMinX.Minimum = 0;
            numPlotMinX.Maximum = 100000;
            numPlotMinX.DecimalPlaces = 2;   // 소수점 둘째 자리까지 정밀 제어 (ps 단위 고려)
            numPlotMinX.Increment = 1M;      // 스핀업 한 단계당 1 ns씩 조절
            numPlotMinX.Value = 0;

            numPlotMaxX.Minimum = 0;
            numPlotMaxX.Maximum = 100000;
            numPlotMaxX.DecimalPlaces = 2;
            numPlotMaxX.Increment = 1M;
            numPlotMaxX.Value = 200;         // 초기 디스플레이 윈도우 한계를 200 ns 부근으로 가정
            btnResetXLimits.Text = "X Auto Scale";

            numSyncOffset.Maximum = 99999;
            numSyncOffset.Minimum = -99999;
            numSyncOffset.Increment = 1000;
            numSyncOffset.Value = 0;

            numAcqOffset.Minimum = 0;
            numAcqOffset.Maximum = 100000;
            numAcqOffset.Value = 0;
            numAcqOffset.Increment = 1000;


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
            PicoHarp_DeviceSettings.SyncOffset = (int)numSyncOffset.Value;
            PicoHarp_MeasurementStatus.AcqOffset = (int)numAcqOffset.Value;
            PicoHarp_DeviceSettings.SyncDivider = (int)numSyncDiv.Value;
            PicoHarp_DeviceSettings.Binning = (int)numBinning.Value;
            PicoHarp_DeviceSettings.CFDLevel0 = (int)numCFDLevel0.Value;
            PicoHarp_DeviceSettings.CFDLevel1 = (int)numCFDLevel1.Value;
            PicoHarp_DeviceSettings.CFDZeroCross0 = (int)numCFDZeroCross0.Value;
            PicoHarp_DeviceSettings.CFDZeroCross1 = (int)numCFDZeroCross1.Value;

            btnPM1ZeroAdjust.BackColor = Color.LightGreen;
            btnPM1ZeroAdjust.Text = "Background OFF";

            // ★ 축 스케일 제어 버튼들의 초기 텍스트 가이드 정렬
            btnLogOrLinearScaleX.Text = "X: Switch to Log"; // X축 버튼 명시
            btnLogOrLinearScaleY.Text = "Y: Switch to Log";
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

        /// <summary>
        /// ★ 로그/선형 스케일 토글 버튼 이벤트 처리
        /// </summary>
        private void btnLogOrLinearScaleY_Click(object sender, EventArgs e)
        {
            bool isLogModeY = _chartManager.ToggleScaleModeY();
            btnLogOrLinearScaleY.Text = isLogModeY ? "Y: Switch to Linear" : "Y: Switch to Log";
        }

        private void btnLogOrLinearScaleX_Click(object sender, EventArgs e)
        {
            // 차트 매니저의 X축 토글을 지시하고 바뀐 스케일 상태를 전달받음
            bool isLogModeX = _chartManager.ToggleScaleModeX();
            btnLogOrLinearScaleX.Text = isLogModeX ? "X: Switch to Linear" : "X: Switch to Log";

        }

        private void numPlotMinX_ValueChanged(object? sender, EventArgs e)
        {
            // 최소값이 최대값보다 커지거나 같아지는 논리적 오류 역전 제어
            if (numPlotMinX.Value >= numPlotMaxX.Value)
            {
                numPlotMinX.Value = numPlotMaxX.Value - 0.1M;
                return;
            }

            _chartManager.SetXLimits((double)numPlotMinX.Value, (double)numPlotMaxX.Value);
        }

        private void numPlotMaxX_ValueChanged(object? sender, EventArgs e)
        {
            // 최대값이 최소값보다 작아지거나 같아지는 오류 역전 제어
            if (numPlotMaxX.Value <= numPlotMinX.Value)
            {
                numPlotMaxX.Value = numPlotMinX.Value + 0.1M;
                return;
            }

            _chartManager.SetXLimits((double)numPlotMinX.Value, (double)numPlotMaxX.Value);
        }

        private void btnResetXLimits_Click(object sender, EventArgs e)
        {
            // 1. 차트 매니저 내부 상태 풀고 자동 레이아웃 변환
            _chartManager.ResetXLimitsToAuto();

            // 2. 자동 계산된 차트의 현재 하한/상한 실제 한계값을 역추적해서 컴포넌트에 반영해주면 직관적입니다.
            var actualLimits = this.histogramPlot.Plot.Axes.GetLimits();

            // 수치 강제 주입 중 ValueChanged 이벤트가 중복 호출되어 렌더가 꼬이는 현상 임시 디태치 제어
            numPlotMinX.ValueChanged -= numPlotMinX_ValueChanged;
            numPlotMaxX.ValueChanged -= numPlotMaxX_ValueChanged;

            // X축 모드에 맞추어 실제 물리 시간을 계산하여 NumericUpDown 텍스트박스 창 동기화
            if (btnLogOrLinearScaleX.Text.Contains("Linear")) // 현재 X축이 로그 상태인 경우 식별
            {
                numPlotMinX.Value = (decimal)Math.Max(0, Math.Pow(10, actualLimits.Left));
                numPlotMaxX.Value = (decimal)Math.Pow(10, actualLimits.Right);
            }
            else
            {
                numPlotMinX.Value = (decimal)Math.Max(0, actualLimits.Left);
                numPlotMaxX.Value = (decimal)actualLimits.Right;
            }

            numPlotMinX.ValueChanged += numPlotMinX_ValueChanged;
            numPlotMaxX.ValueChanged += numPlotMaxX_ValueChanged;
        }

    }
}
