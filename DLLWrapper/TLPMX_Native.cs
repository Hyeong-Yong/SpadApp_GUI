using System;
using System.Text;
using System.Runtime.InteropServices;


namespace SpadApp.DLLWrapper
{
    internal static class TLPMX_Native
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
        // Device Info
        // ------------------------------------------------------------

        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
        public static extern int TLPMX_identificationQuery(
            int instrumentHandle,
            StringBuilder manufacturerName,
            StringBuilder deviceName,
            StringBuilder serialNumber,
            StringBuilder firmwareRevision);


        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
        public static extern int TLPMX_getSensorInfo(
    int instrumentHandle,
    StringBuilder sensorName,
    StringBuilder sensorSerialNumber,
    StringBuilder sensorCalibrationMessage,
    ref short sensorType,
    ref short sensorSubtype,
    ref short sensorFlags);

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
            ref double power);

        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
        public static extern int TLPMX_measEnergy(
            int instrumentHandle,
            ref double energy);

        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
        public static extern int TLPMX_measVoltage(
            int instrumentHandle,
            ref double voltage);

        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
        public static extern int TLPMX_measCurrent(
            int instrumentHandle,
            ref double current);

        // ------------------------------------------------------------
        // Configuration
        // ------------------------------------------------------------

        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
        public static extern int TLPMX_setWavelength(
            int instrumentHandle,
            double wavelength);

        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
        public static extern int TLPMX_getWavelength(
            int instrumentHandle,
            short attribute,
            ref double wavelength);

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

}