using System.Runtime.InteropServices;
using System.Text;

namespace SpadApp.DLLWrapper
{
    public static class PicoHarp_Native
    {
#if X64
        const string PHLib = "phlib64";
#else
        const string PHLib = "phlib";
#endif

        public const int MAXDEVNUM = 8;
        public const int PH_ERROR_DEVICE_OPEN_FAIL = -1;
        public const int MODE_HIST = 0;
        public const int HISTCHAN = 65536;
        public const int FLAG_OVERFLOW = 0x0040;
        public const string TargetLibVersion = "3.0"; //this is what this program was written for

        [DllImport(PHLib, CallingConvention = CallingConvention.StdCall)]
        public extern static int PH_GetLibraryVersion(StringBuilder vers);

        [DllImport(PHLib, CallingConvention = CallingConvention.StdCall)]
        public extern static int PH_GetErrorString(StringBuilder errstring, int errcode);

        [DllImport(PHLib, CallingConvention = CallingConvention.StdCall)]
        public extern static int PH_OpenDevice(int devidx, StringBuilder serial);

        [DllImport(PHLib, CallingConvention = CallingConvention.StdCall)]
        public extern static int PH_Initialize(int devidx, int mode);

        [DllImport(PHLib, CallingConvention = CallingConvention.StdCall)]
        public extern static int PH_GetHardwareInfo(int devidx, StringBuilder model, StringBuilder partno, StringBuilder version);

        [DllImport(PHLib, CallingConvention = CallingConvention.StdCall)]
        public extern static int PH_Calibrate(int devidx);

        [DllImport(PHLib, CallingConvention = CallingConvention.StdCall)]
        public extern static int PH_SetSyncDiv(int devidx, int div);

        [DllImport(PHLib, CallingConvention = CallingConvention.StdCall)]
        public extern static int PH_SetInputCFD(int devidx, int channel, int level, int zerocross);

        [DllImport(PHLib, CallingConvention = CallingConvention.StdCall)]
        public extern static int PH_SetBinning(int devidx, int binning);

        [DllImport(PHLib, CallingConvention = CallingConvention.StdCall)]
        public extern static int PH_SetOffset(int devidx, int offset);

        [DllImport(PHLib, CallingConvention = CallingConvention.StdCall)]
        public extern static int PH_GetResolution(int devidx, ref double resolution);

        [DllImport(PHLib, CallingConvention = CallingConvention.StdCall)]
        public extern static int PH_GetCountRate(int devidx, int channel, ref int countrate);

        [DllImport(PHLib, CallingConvention = CallingConvention.StdCall)]
        public extern static int PH_SetStopOverflow(int devidx, int stop_ovfl, uint stopcount);

        [DllImport(PHLib, CallingConvention = CallingConvention.StdCall)]
        public extern static int PH_ClearHistMem(int devidx, int block);

        [DllImport(PHLib, CallingConvention = CallingConvention.StdCall)]
        public extern static int PH_StartMeas(int devidx, int tacq);

        [DllImport(PHLib, CallingConvention = CallingConvention.StdCall)]
        public extern static int PH_StopMeas(int devidx);

        [DllImport(PHLib, CallingConvention = CallingConvention.StdCall)]
        public extern static int PH_CTCStatus(int devidx, ref int ctcstatus);

        [DllImport(PHLib, CallingConvention = CallingConvention.StdCall)]
        public extern static int PH_GetHistogram(
            int devidx,
            [Out] uint[] chcount, // [Out]을 붙여 Unmanaged -> Managed 방향으로만 데이터가 흐르도록 최적화
            int block             // 매뉴얼 v3.0 스펙에 맞춰 clear가 아닌 block으로 수정
        );

        [DllImport(PHLib, CallingConvention = CallingConvention.StdCall)]
        public extern static int PH_GetFlags(int devidx, ref int flags);

        [DllImport(PHLib, CallingConvention = CallingConvention.StdCall)]
        public extern static int PH_CloseDevice(int devidx);



    }
}