
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

        private System.Windows.Forms.Timer powerMeterMonitorTimer1 = new();
        private System.Windows.Forms.Timer powerMeterMonitorTimer2 = new();
        private bool _isPowerMonitoring = false;

        private double _attenuationDb = 30;
        private double _pm1ZeroOffset = 0.0;
        private bool _pm1ZeroEnabled = false;

        // ------------------------------------------------------------
        // PM Timer Monitor
        // ------------------------------------------------------------
        private void OnPowerMeter1Updated(double power)
        {
            // 예시: UI 레이블에 실시간 파워 출력 처리
            lblPowerMeter1.Text = $"PM #1 Power : {power:E3} W";
        }

        private void OnPowerMeter2Updated(double power)
        {
            // lblPower2.Text = $"PM #2 Power : {power:E3} W";
        }


        private async void PmMonitorTimer1_Tick(object? sender, EventArgs e)
        {
            if (!powerMeterController1.IsConnected)
                return;

            try
            {
                double power = await powerMeterController1.ExecuteMeasurementAsync();
                if (_pm1ZeroEnabled)
                {
                    power -= _pm1ZeroOffset;

                    // 음수 방지
                    if (power < 0)
                        power = 0;
                }

                powerMeterController1.MeasurementStatus.CurrentPower = power;

                lblPowerMeter1.Text =
                    $"{power * 1e6:F3}μW";

                UpdatePhotonStatistics();

            }
            catch (Exception ex)
            {
                Log($"PM Monitor Error: {ex.Message}");
            }
        }

        private async void PmMonitorTimer2_Tick(object? sender, EventArgs e)
        {
            if (!powerMeterController2.IsConnected)
                return;

            try
            {
                double power = await powerMeterController2.ExecuteMeasurementAsync();

                lblPowerMeter2.Text = $"{power:E3} W";
            }
            catch (Exception ex)
            {
                Log($"PM Monitor Error: {ex.Message}");
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


                // --------------------------------------------------------
                // calculate
                // --------------------------------------------------------
                double wavelength = powerMeterController1.MeasurementStatus.CurrentWavelength;

                PhotonStatistics stat = PhotonCalculator.Calculate(power, repetitionRate, detectedPhotonRate, _attenuationDb, wavelength);

                // --------------------------------------------------------
                // UI update
                // --------------------------------------------------------

                lblPhotonFlux.Text = $"{stat.PhotonFlux:E3} photons/s";
                lblMeanPerPulse.Text = $"{stat.MeanPerPulse:F6}";
                lblPDE.Text = $"{stat.PDE:P2}";
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