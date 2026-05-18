using SpadApp.DLLWrapper;
using SpadApp.Parameters;
using System;
using System.Threading.Tasks;

namespace SpadApp.Controller
{
    public class DeviceController_PM100USB
    {
        // ------------------------------------------------------------
        // Device Object
        // ------------------------------------------------------------
        private readonly TLPMXDevice _device = new();

        // ------------------------------------------------------------
        // Device Info
        // ------------------------------------------------------------
        public PM100_DeviceInfo DeviceInfo { get; set; } = new PM100_DeviceInfo();
        public PM100_MeasurementStatus MeasurementStatus = new();

        // ------------------------------------------------------------
        // Timer & Events (🌟 내부 타이머 및 이벤트 추가)
        // ------------------------------------------------------------
        private readonly System.Windows.Forms.Timer _monitorTimer;

        /// <summary>
        /// 파워 측정값이 실시간으로 업데이트될 때 발생할 이벤트
        /// </summary>
        public event Action<double>? PowerUpdated;

        // ------------------------------------------------------------
        // Constructor (🌟 생성자에서 타이머 초기화 및 틱 이벤트 연결)
        // ------------------------------------------------------------
        public DeviceController_PM100USB()
        {
            _monitorTimer = new System.Windows.Forms.Timer();
            _monitorTimer.Interval = 100;
            _monitorTimer.Tick += MonitorTimer_Tick;
        }

        // ------------------------------------------------------------
        // Monitor Timer Tick (🌟 내부 틱 핸들러 구현)
        // ------------------------------------------------------------
        private void MonitorTimer_Tick(object? sender, EventArgs e)
        {
            if (!_device.IsConnected) return;

            try
            {
                // 실시간 파워 측정
                double power = _device.MeasurePower();

                // 글로벌 상태 모델 업데이트
                MeasurementStatus.CurrentPower = power;
                MeasurementStatus.LastUpdateTime = DateTime.Now.ToString("HH:mm:ss.fff");

                // UI 레이어로 업데이트 이벤트 발행
                PowerUpdated?.Invoke(power);
            }
            catch
            {
                // 예외 발생 시 타이머 루프 안정성을 위해 무시하거나 로그 처리
            }
        }

        // ------------------------------------------------------------
        // Connect (🌟 외부 Timer 매개변수 제거)
        // ------------------------------------------------------------
        public bool ConnectDevice(
            int deviceIndex,
            Action<string> logAction)
        {
            try
            {
                string[] resources = TLPMXDevice.FindResources();

                if (resources.Length == 0)
                {
                    logAction("No PM100USB device found.");
                    return false;
                }

                if (deviceIndex >= resources.Length)
                {
                    logAction("Invalid PM100 device index.");
                    return false;
                }

                string resource = resources[deviceIndex];

                // 실제 장비 연결
                _device.Connect(resource);

                // Save Basic Info
                DeviceInfo.IsConnected = true;
                DeviceInfo.DeviceIndex = deviceIndex;
                DeviceInfo.ResourceName = resource;

                // Device Info
                var info = _device.GetDeviceInfo();
                DeviceInfo.Manufacturer = info.manufacturer;
                DeviceInfo.Device = info.model;
                DeviceInfo.SerialNumber = info.serial;
                DeviceInfo.FirmwareVersion = info.firmware;

                // Sensor Info
                _device.GetSensorInfo(
                    out string sensorName,
                    out string sensorSerial,
                    out string calibrationMessage,
                    out short sensorType,
                    out short sensorSubType,
                    out short flags);

                DeviceInfo.SensorName = sensorName;
                DeviceInfo.SensorSerial = sensorSerial;
                DeviceInfo.SensorType = sensorType;
                DeviceInfo.SensorSubType = sensorSubType;
                DeviceInfo.SensorFlags = flags;
                DeviceInfo.CalibrationMessage = calibrationMessage;

                // Wavelength
                MeasurementStatus.CurrentWavelength = _device.GetWavelength();

                // Logging
                logAction($"PM100 Connected : {resource}");
                logAction($"Model : {DeviceInfo.Device}");
                logAction($"Serial : {DeviceInfo.SerialNumber}");
                logAction($"Sensor : {DeviceInfo.SensorName}");
                logAction($"Sensor Serial : {DeviceInfo.SensorSerial}");
                logAction($"Wavelength : {MeasurementStatus.CurrentWavelength} nm");

                // 🌟 내부 타이머 시작
                StartMonitoring();

                return true;
            }
            catch (Exception ex)
            {
                logAction($"PM100 Connect Error : {ex.Message}");
                return false;
            }
        }

        // ------------------------------------------------------------
        // Disconnect
        // ------------------------------------------------------------
        public void DisconnectDevice(Action<string> logAction)
        {
            try
            {
                StopMonitoring();

                _device.Disconnect();
                DeviceInfo.IsConnected = false;

                logAction($"PM100 Disconnected : {DeviceInfo.SerialNumber}");
            }
            catch (Exception ex)
            {
                logAction($"Disconnect Error : {ex.Message}");
            }
        }

        public void DisconnectDevice()
        {
            try
            {
                StopMonitoring();
                _device.Disconnect();
                DeviceInfo.IsConnected = false;
            }
            catch { }
        }

        // ------------------------------------------------------------
        // Measurement
        // ------------------------------------------------------------
        public async Task<double> ExecuteMeasurementAsync()
        {
            return await Task.Run(() =>
            {
                double power = _device.MeasurePower();
                MeasurementStatus.CurrentPower = power;
                MeasurementStatus.LastUpdateTime = DateTime.Now.ToString("HH:mm:ss.fff");
                return power;
            });
        }

        // ------------------------------------------------------------
        // Continuous Measurement
        // ------------------------------------------------------------
        public async Task ContinuousMeasurementAsync(
            Func<bool> checkContinue,
            Action<double>? callback = null,
            int intervalMs = 100)
        {
            await Task.Run(async () =>
            {
                while (checkContinue())
                {
                    try
                    {
                        double power = _device.MeasurePower();
                        MeasurementStatus.CurrentPower = power;
                        MeasurementStatus.LastUpdateTime = DateTime.Now.ToString("HH:mm:ss.fff");

                        callback?.Invoke(power);
                    }
                    catch { }

                    await Task.Delay(intervalMs);
                }
            });
        }

        // ------------------------------------------------------------
        // Wavelength
        // ------------------------------------------------------------
        public void SetWavelength(double wavelengthNm)
        {
            _device.SetWavelength(wavelengthNm);
            MeasurementStatus.CurrentWavelength = wavelengthNm;
        }

        public double GetWavelength()
        {
            double wl = _device.GetWavelength();
            MeasurementStatus.CurrentWavelength = wl;
            return wl;
        }

        // ------------------------------------------------------------
        // Monitor Control (🌟 외부 제어 기능 제공)
        // ------------------------------------------------------------
        public void StartMonitoring() => _monitorTimer.Start();
        public void StopMonitoring() => _monitorTimer.Stop();

        // ------------------------------------------------------------
        // Helpers
        // ------------------------------------------------------------
        public bool IsConnected => _device.IsConnected;
    }
}