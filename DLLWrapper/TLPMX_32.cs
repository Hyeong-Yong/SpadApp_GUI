using System;
using System.Text;
using System.Runtime.InteropServices;


namespace SpadApp.DLLWrapper
{
    internal static class TLPMXNative
    {
        private const string DllName = "TLPMX_32.dll";

        // VISA constants
        public const int VI_SUCCESS = 0;

        // ------------------------------------------------------------
        // Resource Discovery
        // ------------------------------------------------------------

        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
        public static extern int TLPMX_findRsrc(
            int instrumentHandle,
            ref uint resourceCount);

        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
        public static extern int TLPMX_getRsrcName(
            int instrumentHandle,
            uint index,
            StringBuilder resourceName);

        // ------------------------------------------------------------
        // Initialization
        // ------------------------------------------------------------

        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
        public static extern int TLPMX_init(
            string resourceName,
            bool idQuery,
            bool resetDevice,
            ref int instrumentHandle);

        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
        public static extern int TLPMX_close(
            int instrumentHandle);

        // ------------------------------------------------------------
        // Measurement
        // ------------------------------------------------------------

        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
        public static extern int TLPMX_measPower(
            int instrumentHandle,
            ref double power,
            ushort channel);

        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
        public static extern int TLPMX_measEnergy(
            int instrumentHandle,
            ref double energy,
            ushort channel);

        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
        public static extern int TLPMX_measVoltage(
            int instrumentHandle,
            ref double voltage,
            ushort channel);

        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
        public static extern int TLPMX_measCurrent(
            int instrumentHandle,
            ref double current,
            ushort channel);

        // ------------------------------------------------------------
        // Configuration
        // ------------------------------------------------------------

        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
        public static extern int TLPMX_setWavelength(
            int instrumentHandle,
            double wavelength,
            ushort channel);

        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
        public static extern int TLPMX_getWavelength(
            int instrumentHandle,
            ref double wavelength,
            ushort attribute,
            ushort channel);

        // ------------------------------------------------------------
        // Error Handling
        // ------------------------------------------------------------

        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
        public static extern int TLPMX_errorMessage(
            int instrumentHandle,
            int statusCode,
            StringBuilder description);
    }

    public class TLPMXException : Exception
    {
        public int ErrorCode { get; }

        public TLPMXException(int errorCode, string message)
            : base($"TLPMX Error {errorCode}: {message}")
        {
            ErrorCode = errorCode;
        }
    }

    public sealed class TLPMXDevice : IDisposable
    {
        private int _handle;
        private bool _connected;

        public bool IsConnected => _connected;

        // ------------------------------------------------------------
        // Static Helper
        // ------------------------------------------------------------

        public static string[] FindResources()
        {
            int tempHandle = 0;

            uint count = 0;

            CheckError(
                TLPMXNative.TLPMX_findRsrc(tempHandle, ref count),
                tempHandle);

            string[] resources = new string[count];

            for (uint i = 0; i < count; i++)
            {
                StringBuilder sb = new StringBuilder(1024);

                CheckError(
                    TLPMXNative.TLPMX_getRsrcName(tempHandle, i, sb),
                    tempHandle);

                resources[i] = sb.ToString();
            }

            return resources;
        }

        // ------------------------------------------------------------
        // Connect / Disconnect
        // ------------------------------------------------------------

        public void Connect(string resourceName)
        {
            if (_connected)
                return;

            int result = TLPMXNative.TLPMX_init(
                resourceName,
                true,
                false,
                ref _handle);

            CheckError(result, _handle);

            _connected = true;
        }

        public void Disconnect()
        {
            if (!_connected)
                return;

            TLPMXNative.TLPMX_close(_handle);

            _connected = false;
            _handle = 0;
        }

        // ------------------------------------------------------------
        // Measurements
        // ------------------------------------------------------------

        public double MeasurePower(ushort channel = 1)
        {
            double value = 0;

            CheckError(
                TLPMXNative.TLPMX_measPower(
                    _handle,
                    ref value,
                    channel),
                _handle);

            return value;
        }

        public double MeasureEnergy(ushort channel = 1)
        {
            double value = 0;

            CheckError(
                TLPMXNative.TLPMX_measEnergy(
                    _handle,
                    ref value,
                    channel),
                _handle);

            return value;
        }

        public double MeasureVoltage(ushort channel = 1)
        {
            double value = 0;

            CheckError(
                TLPMXNative.TLPMX_measVoltage(
                    _handle,
                    ref value,
                    channel),
                _handle);

            return value;
        }

        public double MeasureCurrent(ushort channel = 1)
        {
            double value = 0;

            CheckError(
                TLPMXNative.TLPMX_measCurrent(
                    _handle,
                    ref value,
                    channel),
                _handle);

            return value;
        }

        // ------------------------------------------------------------
        // Wavelength
        // ------------------------------------------------------------

        public void SetWavelength(double wavelengthNm, ushort channel = 1)
        {
            CheckError(
                TLPMXNative.TLPMX_setWavelength(
                    _handle,
                    wavelengthNm,
                    channel),
                _handle);
        }

        public double GetWavelength(ushort channel = 1)
        {
            double wavelength = 0;

            const ushort TLPM_ATTR_SET_VAL = 0;

            CheckError(
                TLPMXNative.TLPMX_getWavelength(
                    _handle,
                    ref wavelength,
                    TLPM_ATTR_SET_VAL,
                    channel),
                _handle);

            return wavelength;
        }

        // ------------------------------------------------------------
        // Error Handling
        // ------------------------------------------------------------

        private static void CheckError(int status, int handle)
        {
            if (status >= TLPMXNative.VI_SUCCESS)
                return;

            StringBuilder sb = new StringBuilder(1024);

            TLPMXNative.TLPMX_errorMessage(
                handle,
                status,
                sb);

            throw new TLPMXException(status, sb.ToString());
        }

        // ------------------------------------------------------------
        // IDisposable
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