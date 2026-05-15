namespace SpadApp.Parameters
{
    /// <summary>
    /// PM100USB Device Information
    /// 장비 1대의 상태 저장용
    /// </summary>
    public class PM100_DeviceInfo
    {
        // ------------------------------------------------------------
        // Connection
        // ------------------------------------------------------------

        public bool IsConnected { get; set; } = false;

        public int DeviceIndex { get; set; } = -1;

        // ------------------------------------------------------------
        // Resource
        // ------------------------------------------------------------

        public string ResourceName { get; set; }
            = string.Empty;

        // ------------------------------------------------------------
        // Device Information
        // ------------------------------------------------------------

        public string Manufacturer { get; set; }
            = string.Empty;
        public string CalibrationMessage { get; set; }
    = string.Empty;

        public string Device { get; set; }
            = string.Empty;

        public string SerialNumber { get; set; }
            = string.Empty;

        public string FirmwareVersion { get; set; }
            = string.Empty;

        public string DriverVersion { get; set; }
            = string.Empty;

        // ------------------------------------------------------------
        // Sensor Information
        // ------------------------------------------------------------

        public string SensorName { get; set; }
            = string.Empty;

        public string SensorSerial { get; set; }
            = string.Empty;

        public short SensorType { get; set; }

        public short SensorSubType { get; set; }

        public short SensorFlags { get; set; }

    }
}