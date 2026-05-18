
using SpadApp.Controller;
using SpadApp.Model;
using SpadApp.Parameters;
using SpadApp.Utility;

namespace SpadApp
{
    public partial class ucMainView
    {
        // ------------------------------------------------------------
        // PM100USB
        // ------------------------------------------------------------
        public DeviceController_PM100USB powerMeterController1 = new();
        public DeviceController_PM100USB powerMeterController2 = new();

        public event Action<double>? PhotonFluxUpdated;

        private double _attenuationDb = 30;
        private double _pm1ZeroOffset = 0.0;
        private bool _pm1ZeroEnabled = false;

        // ------------------------------------------------------------
        // PM Timer Monitor
        // ------------------------------------------------------------
        /// <summary>
        /// 파워미터 1번의 내부 타이머가 데이터를 읽어올 때마다 실시간 실행됩니다.
        /// </summary>
        private void OnPowerMeter1Updated(double power)
        {
            try
            {
                // 1. Zero Offset 적용 (Background ON 상태일 때)
                if (_pm1ZeroEnabled)
                {
                    power -= _pm1ZeroOffset;

                    // 음수 방지
                    if (power < 0)
                        power = 0;
                }

                // 2. 컨트롤러 모델 상태 동기화
                powerMeterController1.MeasurementStatus.CurrentPower = power;

                // 3. UI 업데이트
                lblPowerMeter1.Text = $"{power * 1e6:F3}μW";

                // 4. 광자 통계 및 ucPDE 연동 이벤트 처리
                UpdatePhotonStatistics();
            }
            catch (Exception ex)
            {
                Log($"PM1 Event Handling Error: {ex.Message}");
            }
        }

        /// <summary>
        /// 파워미터 2번의 내부 타이머가 데이터를 읽어올 때마다 실시간 실행됩니다.
        /// </summary>
        private void OnPowerMeter2Updated(double power)
        {
            try
            {
                lblPowerMeter2.Text = $"{power:E3} W";
            }
            catch (Exception ex)
            {
                Log($"PM2 Event Handling Error: {ex.Message}");
            }
        }


        private void numPMWavelength_ValueChanged(object sender, EventArgs e) {
            if (powerMeterController1.IsConnected == false) return;
            powerMeterController1.MeasurementStatus.CurrentWavelength = (double)numPMWavelength.Value;

            double wavelength = powerMeterController1.MeasurementStatus.CurrentWavelength;
            powerMeterController1.SetWavelength(wavelength);

            double A = powerMeterController1.GetWavelength();
            Log($"Wavelength set to {A} nm");
        }

        private void UpdatePhotonStatistics()
        {
            try
            {
                double repetitionRate = PicoHarp_MeasurementStatus.CountRate0;
                double detectedPhotonRate = PicoHarp_MeasurementStatus.CountRate1;
                double power = powerMeterController1.MeasurementStatus.CurrentPower;
                double wavelength = powerMeterController1.MeasurementStatus.CurrentWavelength;

                PhotonStatistics stat = PhotonCalculator.Calculate(power, repetitionRate, detectedPhotonRate, _attenuationDb, wavelength);

                lblPhotonFlux.Text = $"{stat.PhotonFlux:E3} photons/s";
                lblMeanPerPulse.Text = $"{stat.MeanPerPulse:F6}";
                lblPDE.Text = $"{stat.PDE:P2}";

                // 🌟 2. 값이 계산될 때마다 이벤트를 구독 중인 객체(MainForm -> ucPDE)로 실시간 토스
                PhotonFluxUpdated?.Invoke(stat.PhotonFlux);
            }
            catch (Exception ex)
            {
                Log($"Photon Calc Error : {ex.Message}");
            }
        }

        private async void btnPM1ZeroAdjust_Click(object sender, EventArgs e)
        {
            if (!powerMeterController1.IsConnected)
                return;

            try
            {
                // ------------------------------------------------------------
                // OFF -> ON
                // ------------------------------------------------------------

                if (!_pm1ZeroEnabled)
                {
                    double sum = 0;

                    // 평균으로 안정적인 background 측정
                    for (int i = 0; i < 20; i++)
                    {
                        sum += await powerMeterController1.ExecuteMeasurementAsync();

                        await Task.Delay(20);
                    }

                    _pm1ZeroOffset = sum / 20.0;

                    _pm1ZeroEnabled = true;

                    btnPM1ZeroAdjust.BackColor = Color.IndianRed;
                    btnPM1ZeroAdjust.Text = "Background ON";

                    Log($"PM1 Zero Adjust ON : {_pm1ZeroOffset:E3} W");
                }

                // ------------------------------------------------------------
                // ON -> OFF (Clear)
                // ------------------------------------------------------------

                else
                {
                    _pm1ZeroOffset = 0.0;

                    _pm1ZeroEnabled = false;

                    btnPM1ZeroAdjust.BackColor = Color.LightGreen;
                    btnPM1ZeroAdjust.Text = "Zero OFF";

                    Log("PM1 Zero Adjust OFF");
                }
            }
            catch (Exception ex)
            {
                Log($"Zero Adjust Error : {ex.Message}");
            }
        }

    }
}