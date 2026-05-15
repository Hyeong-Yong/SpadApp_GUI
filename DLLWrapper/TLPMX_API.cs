using System;
using System.Text;

namespace SpadApp.DLLWrapper
{
    public sealed class TLPMXDevice : IDisposable
    {
        private int _handle;
        private bool _connected;

        public bool IsConnected => _connected;

        // ------------------------------------------------------------
        // Resource Search
        // ------------------------------------------------------------

        public static string[] FindResources()
        {
            int tempHandle = 0;

            uint count = 0;

            TLPMX_check(
                TLPMX_Native.TLPMX_findRsrc(
                    tempHandle,
                    ref count),
                tempHandle);

            string[] resources = new string[count];

            for (uint i = 0; i < count; i++)
            {
                StringBuilder sb = new(1024);

                TLPMX_check(
                    TLPMX_Native.TLPMX_getRsrcName(
                        tempHandle,
                        i,
                        sb),
                    tempHandle);

                resources[i] = sb.ToString();
            }

            return resources;
        }

        // ------------------------------------------------------------
        // Connect
        // ------------------------------------------------------------

        public void Connect(string resourceName)
        {
            if (_connected)
                return;

            int result = TLPMX_Native.TLPMX_init(
                resourceName,
                true,
                false,
                ref _handle);

            TLPMX_check(result, _handle);

            _connected = true;
        }

        public void Disconnect()
        {
            if (!_connected)
                return;

            TLPMX_Native.TLPMX_close(_handle);

            _connected = false;
            _handle = 0;
        }

        // ------------------------------------------------------------
        // Device Info
        // ------------------------------------------------------------

        public (string manufacturer,
                string model,
                string serial,
                string firmware)
            GetDeviceInfo()
        {
            StringBuilder manufacturer = new(256);
            StringBuilder model = new(256);
            StringBuilder serial = new(256);
            StringBuilder firmware = new(256);

            TLPMX_check(
                TLPMX_Native.TLPMX_identificationQuery(
                    _handle,
                    manufacturer,
                    model,
                    serial,
                    firmware),
                _handle);

            return (
                manufacturer.ToString(),
                model.ToString(),
                serial.ToString(),
                firmware.ToString());
        }

        // ------------------------------------------------------------
        // Sensor Info
        // ------------------------------------------------------------
        public void GetSensorInfo(
    out string sensorName,
    out string sensorSerial,
    out string calibrationMessage,
    out short sensorType,
    out short sensorSubType,
    out short flags)
        {
            StringBuilder name =
                new StringBuilder(256);

            StringBuilder serial =
                new StringBuilder(256);

            StringBuilder calibration =
                new StringBuilder(256);

            short type = 0;
            short subtype = 0;
            short sensorFlags = 0;

            TLPMX_check(
                TLPMX_Native.TLPMX_getSensorInfo(
                    _handle,
                    name,
                    serial,
                    calibration,
                    ref type,
                    ref subtype,
                    ref sensorFlags),
                _handle);

            sensorName = name.ToString();
            sensorSerial = serial.ToString();
            calibrationMessage = calibration.ToString();
            sensorType = type;
            sensorSubType = subtype;
            flags = sensorFlags;
        }

        // ------------------------------------------------------------
        // Measurement
        // ------------------------------------------------------------

        public double MeasurePower()
        {
            double value = 0;

            TLPMX_check(
                TLPMX_Native.TLPMX_measPower(
                    _handle,
                    ref value),
                _handle);

            return value;
        }

        public double MeasureEnergy()
        {
            double value = 0;

            TLPMX_check(
                TLPMX_Native.TLPMX_measEnergy(
                    _handle,
                    ref value),
                _handle);

            return value;
        }

        // ------------------------------------------------------------
        // Wavelength
        // ------------------------------------------------------------

        public void SetWavelength(
            double wavelengthNm)
        {
            TLPMX_check(
                TLPMX_Native.TLPMX_setWavelength(
                    _handle,
                    wavelengthNm),
                _handle);
        }

        public double GetWavelength()
        {
            double wavelength = 0;

            const short TLPM_ATTR_SET_VAL = 0;

            TLPMX_check(
                TLPMX_Native.TLPMX_getWavelength(
                    _handle,
                    TLPM_ATTR_SET_VAL,
                    ref wavelength),
                _handle);

            return wavelength;
        }

        // ------------------------------------------------------------
        // Error
        // ------------------------------------------------------------

        private static void TLPMX_check(
            int status,
            int handle)
        {
            if (status >= TLPMX_Native.VI_SUCCESS)
                return;

            StringBuilder sb = new(1024);

            TLPMX_Native.TLPMX_errorMessage(
                handle,
                status,
                sb);

            throw new TLPMXException(
                status,
                sb.ToString());
        }

        // ------------------------------------------------------------
        // Dispose
        // ------------------------------------------------------------

        public void Dispose()
        {
            Disconnect();
            GC.SuppressFinalize(this);
        }

        ~TLPMXDevice()
        {
            Disconnect();
        }
    }
}