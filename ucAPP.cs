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
        private HistogramChartManager _chartManager;

        private PtuT2DataConverter _ptuConverter = new PtuT2DataConverter();

        public ucAPP(DeviceController_PicoHarp300 tcspcController)
        {
            InitializeComponent();
            _tcspcController = tcspcController;

            _chartManager = new HistogramChartManager(this.APPplot)
            {
                ChartTitle = "SPAD Correlation (Afterpulse / Crosstalk) Distribution"
            };

            // 애프터펄스는 로그 스케일 관측이 기본이므로 생성되자마자 Y축 로그 모드 활성화
            _chartManager.ToggleScaleModeY();
        }

        private void ucAPP_Load(object sender, EventArgs e)
        {
            // 프로그레스바 초기화
            progressBarPtu.Minimum = 0;
            progressBarPtu.Maximum = 100;
            progressBarPtu.Value = 0;

            // 컨버터 이벤트 바인딩
            _ptuConverter.ConversionProgressChanged += OnPtuProgressChanged;
            _ptuConverter.StatusMessageReceived += OnPtuStatusMessage;

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

            btnAPPmeasure.Enabled = true;
            btnAPPmeasure.Text = "Measure";
            btnAPPmeasure.BackColor = SystemColors.Control;

            // (가정) UI 디자이너 창에서 추가해야 할 컨트롤 기본 상태
            chkSavePtuFile.Checked = false;
        }

        // 🔄 프로그레스 바 스레드 안전 업데이트 (Invoke 패턴)
        private void OnPtuProgressChanged(object? sender, PtuProgressEventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action(() => OnPtuProgressChanged(sender, e)));
                return;
            }

            // ProgressBar 값 업데이트
            progressBarPtu.Value = (int)Math.Min(100, Math.Max(0, e.ProgressPercentage));

            // (선택) 진행률 텍스트 업데이트
            // lblProgress.Text = $"Processing: {e.ProgressPercentage:F1}% ({e.CurrentRecord:N0} / {e.TotalRecords:N0})";
        }

        private void OnPtuStatusMessage(object? sender, string message)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action(() => OnPtuStatusMessage(sender, message)));
                return;
            }

            // (선택) 상태창이나 콘솔에 로그 출력
            Console.WriteLine($"[PTU Analyzer] {message}");
        }

        private void btnAPPsettings_Click(object sender, EventArgs e)
        {
            // 🌟 중복되었던 IsConnected 검사 블록 1개 삭제 처리됨
            if (!_tcspcController.IsConnected)
            {
                MessageBox.Show("PicoHarp 300 장비 연결 상태를 먼저 확인해 주세요.", "알림", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 1. 하드웨어 모드 재초기화 및 파라미터 복구
            _tcspcController.InitializeMode(PicoHarpDevice.MODE_T2);
            _tcspcController.UpdateDeviceSettings();

            // 2. 프로세서 생성 및 이벤트 바인딩
            _processor = new TttrCorrelationProcessor(_tcspcController, maxDelayNs: 10000, binWidthNs: 1);
            _processor.ReferenceChannel = 1; // SPAD 채널
            _processor.TargetChannel = 1;    // 동일한 SPAD 채널

            // ★ UI 연동 파트: 측정 모드 라디오 버튼 읽기 (컨트롤 이름은 디자인에 맞게 변경하세요)
            _processor.Mode = radEnableModeAutocorr.Checked ? CorrelationMode.Autocorrelation : CorrelationMode.InterArrival;

            // ★ UI 연동 파트: 디스크 스트리밍 옵션 읽기
            _processor.EnableDataSaving = chkSavePtuFile.Checked;
            if (_processor.EnableDataSaving)
            {
                // 현재 시간 기반으로 파일명 자동 생성 (예: Data_20260521_120100.ptu)
                string fileName = $"Data_{DateTime.Now:yyyyMMdd_HHmmss}.ptu";
                _processor.SaveFilePath = Path.Combine(Application.StartupPath, "Data", fileName);
                Directory.CreateDirectory(Path.GetDirectoryName(_processor.SaveFilePath)!);
            }

            _processor.ProgressUpdated += OnProcessorProgressUpdated;
            _processor.StatusMessageLogged += OnProcessorLogReceived;
            _processor.MeasurementFinished += OnProcessorMeasurementFinished;
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

                _processor.EnableDataSaving = chkSavePtuFile.Checked; // 체크박스 상태 대입
                if (_processor.EnableDataSaving)
                {
                    // 체크되어 있다면 현재 시간을 바탕으로 겹치지 않는 새 파일명 생성
                    string timeStamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                    string fileName = $"SPAD_Data_{timeStamp}.ptu";

                    // 프로그램 실행 폴더 밑에 "Data" 폴더를 만들고 거기에 저장
                    string dataFolder = Path.Combine(Application.StartupPath, "Data");
                    Directory.CreateDirectory(dataFolder); // 폴더가 없으면 자동 생성

                    _processor.SaveFilePath = Path.Combine(dataFolder, fileName);

                    // UI 로그로 저장 위치 알림 (선택 사항)
                    Console.WriteLine($"[저장 모드 활성화] 파일 경로: {_processor.SaveFilePath}");
                }
                else
                {
                    // 체크가 풀려있다면 경로를 지우고 SSD 저장 없이 RAM 모니터링만 수행
                    _processor.SaveFilePath = "";
                    Console.WriteLine("[검증 모드] 디스크 저장 없이 고속 모니터링만 수행합니다.");
                }
                // =========================================================================
                try
                {
                    // 이 StartAcquisitionAsync 내부에서 EnableDataSaving이 true일 때만 FileStream을 엽니다.
                    await _processor.StartAcquisitionAsync(targetDurationMs);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"계측 중 에러 발생: {ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    ResetUIState();
                    _tcspcController.StartMonitoring();
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

        private async void btnLoadPtu_Click(object sender, EventArgs e)
        {
            if (_processor == null)
            {
                MessageBox.Show("먼저 [Apply Settings]를 눌러 하드웨어/프로세서를 초기화해 주세요.", "알림", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "PTU Files (*.ptu)|*.ptu|All Files (*.*)|*.*";
                ofd.Title = "PTU 데이터 분석 모드 선택";

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    btnLoadPtu.Enabled = false;
                    progressBarPtu.Value = 0;

                    try
                    {
                        this.APPplot.Plot.Clear();

                        // 1. Full-Pair 분석 모드 (고해상도 근거리 분석)
                        if (radBtnFullPairMode.Checked)
                        {
                            double maxDelayNs = (double)numPlotMaxX.Value;
                            double binWidthNs = 10.0; // 10ns 해상도 예시

                            var result = await _processor.AnalyzeHighResAutocorrelationAsync(ofd.FileName, maxDelayNs, binWidthNs);

                            if (result != null)
                            {
                                // 일반 Linear 히스토그램 차트 그리기
                                _chartManager.UpdateFromProcessedData(result, binWidthNs);
                                this.APPplot.Plot.Title("High-Res Full-Pair Autocorrelation");
                                this.APPplot.Plot.XLabel("Delay Time (ns)");
                            }
                        }
                        // 2. Multi-tau 분석 모드 (거시적 전체 영역 분석)
                        else if (radBtnMultiTauMode.Checked)
                        {
                            double baseBinWidthNs = 1000; // 1us 시작
                            int cascades = 24;
                            int binsPerCascade = 16;

                            var result = await _processor.AnalyzeMultiTauAsync(ofd.FileName, baseBinWidthNs, cascades, binsPerCascade);

                            if (result != null)
                            {
                                double[] delays = result.Value.delays;
                                double[] correlations = result.Value.correlations;

                                // 로그 스케일 변환 및 차트 렌더링
                                double[] logDelays = new double[delays.Length];
                                for (int i = 0; i < delays.Length; i++) logDelays[i] = Math.Log10(delays[i]);

                                var scatter = this.APPplot.Plot.Add.Scatter(logDelays, correlations);

                                // X축 지수 표기 설정
                                ScottPlot.TickGenerators.NumericAutomatic tickGenX = new ScottPlot.TickGenerators.NumericAutomatic();
                                tickGenX.LabelFormatter = x => Math.Pow(10, x).ToString("G3");
                                this.APPplot.Plot.Axes.Bottom.TickGenerator = tickGenX;

                                this.APPplot.Plot.Title("Multi-Tau Autocorrelation (Log-Scale)");
                                this.APPplot.Plot.XLabel("Delay Time \u03C4 (ns)");
                            }
                        }

                        this.APPplot.Plot.YLabel("Correlation G(\u03C4) - 1");
                        this.APPplot.Plot.Axes.AutoScale();
                        this.APPplot.Refresh();

                        MessageBox.Show("분석이 완료되었습니다.", "완료", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"분석 오류: {ex.Message}", "에러", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    finally
                    {
                        btnLoadPtu.Enabled = true;
                        progressBarPtu.Value = 100;
                    }
                }
            }
        }

        private void radBtnT2Mode_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
}