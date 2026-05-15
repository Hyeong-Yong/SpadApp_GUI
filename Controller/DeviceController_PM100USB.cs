using SpadApp.DLLWrapper;
using SpadApp.Parameters;

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

        public PM100_DeviceInfo DeviceInfo { get; set;}
            = new PM100_DeviceInfo();


        public PM100_MeasurementStatus MeasurementStatus = new();

        // ------------------------------------------------------------
        // Timer
        // ------------------------------------------------------------

        private System.Windows.Forms.Timer? _monitorTimer;

        // ------------------------------------------------------------
        // Connect
        // ------------------------------------------------------------

        public bool ConnectDevice(
            int deviceIndex,
            System.Windows.Forms.Timer timer,
            Action<string> logAction)
        {
            try
            {
                string[] resources =
                    TLPMXDevice.FindResources();

                if (resources.Length == 0)
                {
                    logAction(
                        "No PM100USB device found.");

                    return false;
                }

                if (deviceIndex >= resources.Length)
                {
                    logAction(
                        "Invalid PM100 device index.");

                    return false;
                }

                string resource =
                    resources[deviceIndex];

                // 실제 장비 연결
                _device.Connect(resource);

                // ----------------------------------------------------
                // Save Basic Info
                // ----------------------------------------------------

                DeviceInfo.IsConnected = true;

                DeviceInfo.DeviceIndex = deviceIndex;

                DeviceInfo.ResourceName = resource;

                // ----------------------------------------------------
                // Device Info
                // ----------------------------------------------------

                var info =
                    _device.GetDeviceInfo();

                DeviceInfo.Manufacturer =
                    info.manufacturer;

                DeviceInfo.Device =
                    info.model;

                DeviceInfo.SerialNumber =
                    info.serial;

                DeviceInfo.FirmwareVersion =
                    info.firmware;

                // ----------------------------------------------------
                // Sensor Info
                // ----------------------------------------------------

                _device.GetSensorInfo(
                    out string sensorName,
                    out string sensorSerial,
                    out string calibrationMessage,
                    out short sensorType,
                    out short sensorSubType,
                    out short flags);

                DeviceInfo.SensorName =
                    sensorName;

                DeviceInfo.SensorSerial =
                    sensorSerial;

                DeviceInfo.SensorType =
                    sensorType;

                DeviceInfo.SensorSubType =
                    sensorSubType;

                DeviceInfo.SensorFlags =
                    flags;

                DeviceInfo.CalibrationMessage =
    calibrationMessage;

                // ----------------------------------------------------
                // Wavelength
                // ----------------------------------------------------

                MeasurementStatus.CurrentWavelength =
                    _device.GetWavelength();

                // ----------------------------------------------------
                // Logging
                // ----------------------------------------------------

                logAction(
                    $"PM100 Connected : {resource}");

                logAction(
                    $"Model : {DeviceInfo.Device}");

                logAction(
                    $"Serial : {DeviceInfo.SerialNumber}");

                logAction(
                    $"Sensor : {DeviceInfo.SensorName}");

                logAction(
                    $"Sensor Serial : {DeviceInfo.SensorSerial}");

                logAction(
                    $"Wavelength : {MeasurementStatus.CurrentWavelength} nm");

                // ----------------------------------------------------
                // Timer
                // ----------------------------------------------------

                _monitorTimer = timer;

                _monitorTimer.Interval = 100;

                _monitorTimer.Start();

                return true;
            }
            catch (Exception ex)
            {
                logAction(
                    $"PM100 Connect Error : {ex.Message}");

                return false;
            }
        }

        // ------------------------------------------------------------
        // Disconnect
        // ------------------------------------------------------------

        public void DisconnectDevice(
            Action<string> logAction)
        {
            try
            {
                _monitorTimer?.Stop();

                _device.Disconnect();

                DeviceInfo.IsConnected = false;

                logAction(
                    $"PM100 Disconnected : {DeviceInfo.SerialNumber}");
            }
            catch (Exception ex)
            {
                logAction(
                    $"Disconnect Error : {ex.Message}");
            }
        }

        // ------------------------------------------------------------
        // Measurement
        // ------------------------------------------------------------

        public async Task<double>
            ExecuteMeasurementAsync()
        {
            return await Task.Run(() =>
            {
                double power =
                    _device.MeasurePower();

                MeasurementStatus.CurrentPower =
                    power;

                MeasurementStatus.LastUpdateTime =
                    DateTime.Now.ToString(
                        "HH:mm:ss.fff");

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
                        double power =
                            _device.MeasurePower();

                        MeasurementStatus.CurrentPower =
                            power;

                        MeasurementStatus.LastUpdateTime =
                            DateTime.Now.ToString(
                                "HH:mm:ss.fff");

                        callback?.Invoke(power);
                    }
                    catch
                    {

                    }

                    await Task.Delay(intervalMs);
                }
            });
        }

        // ------------------------------------------------------------
        // Wavelength
        // ------------------------------------------------------------

        public void SetWavelength(
            double wavelengthNm)
        {
            _device.SetWavelength(
                wavelengthNm);

            MeasurementStatus.CurrentWavelength =
                wavelengthNm;
        }

        public double GetWavelength()
        {
            double wl =                _device.GetWavelength();

            MeasurementStatus.CurrentWavelength =
                wl;

            return wl;
        }

        // ------------------------------------------------------------
        // Helpers
        // ------------------------------------------------------------

        public bool IsConnected =>
            _device.IsConnected;
    }
}