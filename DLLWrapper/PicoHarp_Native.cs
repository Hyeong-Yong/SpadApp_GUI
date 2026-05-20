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




        public const int HISTCHAN = 65536;
        public const int FLAG_OVERFLOW = 0x0040;
        public const int FLAG_FIFOFULL = 0x0003; //  FiFo 버퍼가 가득 찼음을 알리는 플래그 식별 코드

        public const string TargetLibVersion = "3.0"; //this is what this program was written for



        // ★ 매뉴얼 v3.0 스펙에 따른 Sync Offset 제한값 상수 선언
        public const int SYNCOFFSMIN = -99999;
        public const int SYNCOFFSMAX = 99999;

        // ★ [새로 추가] 물리적 케이블 딜레이(지연)를 하드웨어 레벨에서 보정하는 함수
        [DllImport(PHLib, CallingConvention = CallingConvention.StdCall)]
        public extern static int PH_SetSyncOffset(int devidx, int offset);


        [DllImport(PHLib, CallingConvention = CallingConvention.StdCall)]
        public extern static int PH_GetLibraryVersion(StringBuilder vers);

        [DllImport(PHLib, CallingConvention = CallingConvention.StdCall)]
        public extern static int PH_GetErrorString(StringBuilder errstring, int errcode);

        [DllImport(PHLib, CallingConvention = CallingConvention.StdCall)]
        public extern static int PH_OpenDevice(int devidx, StringBuilder serial);

        /// <summary>
        /// 장비를 초기화합니다. mode 파라미터에 MODE_HIST(0), MODE_T2(2), MODE_T3(3)를 조립하여 전달할 수 있습니다.
        /// </summary>
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

        // ====================================================================================
        // ★ [대단히 중요] TTTR (T2/T3 모드) 및 오리지널 광자 소스 파일(.ptu) 생성을 위해 필수 추가된 함수 영역
        // ====================================================================================

        /// <summary>
        /// 하드웨어의 실시간 FIFO 메모리 버퍼로부터 광자 이벤트 스트리밍 데이터를 읽어옵니다.
        /// </summary>
        [DllImport(PHLib, CallingConvention = CallingConvention.StdCall)]
        public extern static int PH_ReadFiFo(
            int devidx,
            [Out] uint[] buffer,
            int count,
            out int nactual // ★ 누락되었던 4번째 파라미터 추가!
        );


        [DllImport(PHLib, CallingConvention = CallingConvention.StdCall)]
        public extern static int PH_GetFlags(int devidx, ref int flags);

        [DllImport(PHLib, CallingConvention = CallingConvention.StdCall)]
        public extern static int PH_CloseDevice(int devidx);



    }
}