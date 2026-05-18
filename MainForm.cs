using System.Runtime.InteropServices;

namespace SpadApp
{
    public partial class MainForm : Form
    {

        // 현재 어떤 화면이 켜져 있는지 추적하는 변수
        private UserControl _currentView;

        private ucMainView _ucMainView;
        private ucDCR _ucDCRview;
        private ucPDE _ucPDEview;

        private int borderSize = 2;
        private Size formSize; //Keep form size when it is minimized and restored.Since the form is resized because it takes into account the size of the title bar and borders.

        public MainForm()
        {
            InitializeComponent();
            InitializeViews();
            CollapseMenu();
            this.Padding = new Padding(borderSize); //Border size
            this.BackColor = Color.FromArgb(98, 102, 244); //Border color 
        }

        private void InitializeViews()
        {
            // 메인 뷰를 먼저 생성, "TCSPCDeviceController" ucDCR에 주입 및 생성
            _ucMainView = new ucMainView { Dock = DockStyle.Fill };
            _ucDCRview = new ucDCR(_ucMainView.TCSPCDeviceController) { Dock = DockStyle.Fill };
            _ucPDEview = new ucPDE(_ucMainView.TCSPCDeviceController, _ucDCRview) { Dock = DockStyle.Fill };

            // 🌟 ucMainView에서 PhotonFlux 이벤트가 발생하면, ucPDE의 텍스트박스(N_inc)에 실시간 주입
            _ucMainView.PhotonFluxUpdated += (flux) => _ucPDEview.UpdateIncidentPhotonNumber(flux);

            // 메인 패널에 화면 등록
            panelMainView.Controls.Add(_ucMainView);
            panelMainView.Controls.Add(_ucDCRview);
            panelMainView.Controls.Add(_ucPDEview);

            // 초기 숨김 처리
            _ucMainView.Visible = false;
            _ucDCRview.Visible = false;
            _ucPDEview.Visible = false;

            // 시작 화면으로 홈 화면만 켜줍니다.
            ChangeView(_ucMainView);
        }

        private void ChangeView(UserControl newView)
        {
            // 이미 그 화면이 켜져 있다면 아무 변화도 주지 않고 함수 종료
            if (_currentView == newView) return;

            // 기존에 켜져 있던 화면이 있다면 숨김 처리
            if (_currentView != null)
            {
                _currentView.Visible = false;
            }

            // 새 화면을 보임 처리
            newView.Visible = true;

            // 현재 화면 상태 업데이트
            _currentView = newView;
        }
        private void CollapseMenu()
        {
            if (this.panelMenu.Width > 250)
            {
                panelMenu.Width = 100;
                pictureBox1.Visible = false;
                btnMenu.Dock = DockStyle.Top;
                foreach (Button menuButton in panelMenu.Controls.OfType<Button>())
                {
                    menuButton.Text = "";
                    menuButton.ImageAlign = ContentAlignment.MiddleCenter;
                    menuButton.Padding = new Padding(0);
                }
            }
            else
            {
                panelMenu.Width = 300;
                pictureBox1.Visible = true;
                btnMenu.Dock = DockStyle.None;
                foreach (Button menuButton in panelMenu.Controls.OfType<Button>())
                {
                    menuButton.Text = "    " + menuButton.Tag.ToString();
                    menuButton.ImageAlign = ContentAlignment.MiddleLeft;
                    menuButton.Padding = new Padding(10, 15, 15, 0);
                }
            }
        }
        private void MainForm_Load(object sender, EventArgs e) => formSize = this.ClientSize;
        private void btnMainView_Click(object sender, EventArgs e) => ChangeView(_ucMainView);
        private void btnDCRview_Click(object sender, EventArgs e) => ChangeView(_ucDCRview);
        private void btnPDEview_Click(object sender, EventArgs e) => ChangeView(_ucPDEview);
        private void btnMenu_Click(object sender, EventArgs e) => CollapseMenu();
        private void btnClose_Click(object sender, EventArgs e) => this.Close();
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // 🌟 우측 상단 X 버튼, Alt+F4, 혹은 btnClose 클릭 등 '어떤 방식'으로든 
            // 폼이 닫힐 때 하드웨어 연결을 가장 먼저 안전하게 차단합니다.
            try
            {
                if (_ucMainView != null)
                {
                    // 1. PicoHarp300 실시간 모니터링 타이머 중지 및 장비 해제
                    if (_ucMainView.TCSPCDeviceController != null && _ucMainView.TCSPCDeviceController.IsConnected)
                    {
                        _ucMainView.TCSPCDeviceController.DisconnectDevice();
                    }

                    // 2. 파워미터 #1 내부 타이머 중지 및 장비 해제
                    if (_ucMainView.powerMeterController1 != null && _ucMainView.powerMeterController1.IsConnected)
                    {
                        _ucMainView.powerMeterController1.DisconnectDevice();
                    }

                    // 3. 파워미터 #2 내부 타이머 중지 및 장비 해제
                    if (_ucMainView.powerMeterController2 != null && _ucMainView.powerMeterController2.IsConnected)
                    {
                        _ucMainView.powerMeterController2.DisconnectDevice();
                    }
                }
            }
            catch (Exception ex)
            {
                // 종료 시 발생하는 예외가 시스템을 크래시하지 않도록 방어막 형성
                System.Diagnostics.Debug.WriteLine($"FormClosing 하드웨어 해제 중 에러: {ex.Message}");
            }
        }
    }

}