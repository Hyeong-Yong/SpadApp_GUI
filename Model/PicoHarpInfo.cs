using System;
using System.Collections.Generic;
using System.Text;

namespace SpadApp.Model
{
    public static class PicoHarpInfo
    {
        // 외부에서 접근 가능하도록 public으로 선언
        public static StringBuilder LibVer = new StringBuilder(8);
        public static StringBuilder Serial = new StringBuilder(8);
        public static StringBuilder ErrStr = new StringBuilder(40);
        public static StringBuilder Model = new StringBuilder(16);
        public static StringBuilder PartNo = new StringBuilder(8);
        public static StringBuilder Version = new StringBuilder(8);

        // 모든 데이터를 한 번에 초기화하는 메서드를 추가할 수도 있습니다.
        public static void ClearAll()
        {
            LibVer.Clear();
            Serial.Clear();
            ErrStr.Clear();
            Model.Clear();
            PartNo.Clear();
            Version.Clear();
        }

        public static int DeviceIndex { get; set; }

        public static bool IsConnected { get; set; }

        // 추가
        public static  List<int> AvailableDevices { get; set; }
            = new();
    }
}
