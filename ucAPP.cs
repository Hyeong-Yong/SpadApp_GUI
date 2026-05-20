using ScottPlot;
using SpadApp.Controller;
using SpadApp.DLLWrapper;
using SpadApp.Model;
using SpadApp.Parameters;
using SpadApp.View;
using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpadApp
{
    public partial class ucAPP : UserControl
    {
        private readonly DeviceController_PicoHarp300 _tcspcController;

        private TttrCorrelationProcessor? _processor;
        private bool _isAppMeasuring = false;

        // ★ 차트 매니저 선언 (이제 모든 차트 제어는 이 매니저만 담당합니다)
        private HistogramChartManager _chartManager;

        public ucAPP(DeviceController_PicoHarp300 tcspcController)
        {
            InitializeComponent();
            _tcspcController = tcspcController;

            // ★ InitChartLayout() 수동 호출 삭제됨! 차트 매니저가 알아서 초기화합니다.
            _chartManager = new HistogramChartManager(this.APPplot)
            {
                ChartTitle = "SPAD Correlation (Afterpulse / Crosstalk) Distribution"
            };

            // 애프터펄스는 로그 스케일 관측이 기본이므로 생성되자마자 Y축 로그 모드 활성화
            _chartManager.ToggleScaleModeY();
        }

        private void ucAPP_Load(object sender, EventArgs e)
        {
            numAcqTime.Minimum = 1;
            numAcqTime.Maximum = 3600;
            numAcqTime.Value = 10;

            numPlotMinX.Minimum = 0;
            numPlotMinX.Maximum = 100000;
            numPlotMinX.DecimalPlaces = 2;
            numPlotMinX.Increment = 1M;
            numPlotMinX.Value = 0;

            numPlotMaxX.Minimum = 0;
            numPlotMaxX.Maximum = 100000;
            numPlotMaxX.DecimalPlaces = 2;
            numPlotMaxX.Increment = 1M;
            numPlotMaxX.Value = 2000;
            btnResetXLimits.Text = "X Auto Scale";

            radBtnT2Mode.Checked = true;
            radBtnT3Mode.Checked = false;

            btnAPPmeasure.Enabled = true;
            btnAPPmeasure.Text = "Measure";
            btnAPPmeasure.BackColor = SystemColors.Control;
        }

        // ★ 수동 차트 조작 함수인 InitChartLayout()는 삭제되었습니다.

        private void btnAPPsettings_Click(object sender, EventArgs e)
        {
            // 🌟 중복되었던 IsConnected 검사 블록 1개 삭제 처리됨
            if (!_tcspcController.IsConnected)
            {
                MessageBox.Show("PicoHarp 300 장비 연결 상태를 먼저 확인해 주세요.", "알림", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int targetMode = radBtnT2Mode.Checked ? PicoHarpDevice.MODE_T2 : PicoHarpDevice.MODE_T3;

            // 1. 하드웨어 모드 재초기화 및 파라미터 복구
            _tcspcController.InitializeMode(targetMode);
            _tcspcController.UpdateDeviceSettings();

            // 2. 프로세서 생성 및 이벤트 바인딩
            _processor = new TttrCorrelationProcessor(_tcspcController, maxDelayNs: 10000, binWidthNs: 1);
            _processor.ReferenceChannel = 1; // SPAD 채널
            _processor.TargetChannel = 1;    // 동일한 SPAD 채널

            _processor.ProgressUpdated += OnProcessorProgressUpdated;
            _processor.StatusMessageLogged += OnProcessorLogReceived;
            _processor.MeasurementFinished += OnProcessorMeasurementFinished;

            // ★ APPplot.Clear(), PlotData 수동 생성 등 차트 매니저의 영역을 침범하는 코드 삭제됨!
            // 셋팅이 완료되면 계측 버튼을 누를 때 _chartManager가 데이터를 받아 알아서 렌더링합니다.

            MessageBox.Show("하드웨어가 성공적으로 설정되었습니다.", "설정 완료", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private async void btnAPPmeasure_Click(object sender, EventArgs e)
        {
            if (_processor == null)
            {
                MessageBox.Show("먼저 [Apply Settings] 버튼을 클릭하여 모드 초기화를 완료해 주세요.", "알림", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (!_isAppMeasuring)
            {
                _isAppMeasuring = true;
                btnAPPmeasure.Text = "Stop";
                btnAPPmeasure.BackColor = System.Drawing.Color.IndianRed;
                btnAPPsettings.Enabled = false;

                // FIFO 통신 안전성을 위해 메인 타이머 모니터링 정지
                _tcspcController.StopMonitoring();

                int targetDurationMs = (int)numAcqTime.Value * 1000;

                int startRet = _tcspcController.StartMeasurement(targetDurationMs);
                if (startRet != 0)
                {
                    ResetUIState();
                    _tcspcController.StartMonitoring();
                    return;
                }

                // 🌟 try-finally 구조로 변경하여 무조건적으로 하드웨어 모니터링 복구 보장
                try
                {
                    await _processor.StartAcquisitionAsync(targetDurationMs);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"계측 중 에러 발생: {ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    ResetUIState();
                    _tcspcController.StartMonitoring(); // 측정 성공/실패 여부와 관계없이 타이머 복구
                }
            }
            else
            {
                _isAppMeasuring = false;
                if (_processor != null) _processor.StopAcquisition();
                _tcspcController.StopMeasurement();
            }
        }

        // ====================================================================================
        // 비동기 스레드 이벤트 처리단 (Invoke 패턴 가드)
        // ====================================================================================

        private void OnProcessorProgressUpdated(object? sender, TttrProgressEventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action(() => OnProcessorProgressUpdated(sender, e)));
                return;
            }

            lblAPPelapsedTime.Text = $"Elapsed Time: {e.ElapsedSeconds:F2} / {numAcqTime.Value} sec";
            lblAPPtotalRecords.Text = $"Total Records: {e.TotalRecordsReceived:N0} events";

            if (_processor != null)
            {
                // ★ 완벽하게 차트 매니저를 통해서만 차트를 업데이트함
                _chartManager.UpdateFromProcessedData(_processor.CorrelationHistogram, _processor.BinWidthNs);
            }


        }

        private void OnProcessorLogReceived(object? sender, string message)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action(() => OnProcessorLogReceived(sender, message)));
                return;
            }
        }

        private void OnProcessorMeasurementFinished(object? sender, EventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action(() => OnProcessorMeasurementFinished(sender, e)));
                return;
            }
            // try-finally 블록에서 UI 리셋과 타이머 복구를 처리하므로 여기서는 비워둡니다.
        }

        private void ResetUIState()
        {
            _isAppMeasuring = false;
            btnAPPmeasure.Enabled = true;
            btnAPPmeasure.Text = "Measure";
            btnAPPmeasure.BackColor = SystemColors.Control;
            btnAPPsettings.Enabled = true;
        }

        private void btnLogOrLinearScaleY_Click(object sender, EventArgs e)
        {
            bool isLogModeY = _chartManager.ToggleScaleModeY();
            btnLogOrLinearScaleY.Text = isLogModeY ? "Y: Switch to Linear" : "Y: Switch to Log";
        }

        private void btnLogOrLinearScaleX_Click(object sender, EventArgs e)
        {
            bool isLogModeX = _chartManager.ToggleScaleModeX();
            btnLogOrLinearScaleX.Text = isLogModeX ? "X: Switch to Linear" : "X: Switch to Log";
        }

        private void btnResetXLimits_Click(object sender, EventArgs e)
        {
            _chartManager.ResetXLimitsToAuto();
            var actualLimits = this.APPplot.Plot.Axes.GetLimits();

            numPlotMinX.ValueChanged -= numPlotMinX_ValueChanged;
            numPlotMaxX.ValueChanged -= numPlotMaxX_ValueChanged;

            if (btnLogOrLinearScaleX.Text.Contains("Linear"))
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

        private void numPlotMinX_ValueChanged(object? sender, EventArgs e)
        {
            if (numPlotMinX.Value >= numPlotMaxX.Value)
            {
                numPlotMinX.Value = numPlotMaxX.Value - 0.1M;
                return;
            }
            _chartManager.SetXLimits((double)numPlotMinX.Value, (double)numPlotMaxX.Value);
        }

        private void numPlotMaxX_ValueChanged(object? sender, EventArgs e)
        {
            if (numPlotMaxX.Value <= numPlotMinX.Value)
            {
                numPlotMaxX.Value = numPlotMinX.Value + 0.1M;
                return;
            }
            _chartManager.SetXLimits((double)numPlotMinX.Value, (double)numPlotMaxX.Value);
        }
    }
}