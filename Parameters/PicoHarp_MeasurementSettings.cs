using System;
using System.Collections.Generic;
using System.Text;

namespace SpadApp.Parameters
{
    public static class PicoHarp_DeviceSettings
    {
        public static int Binning { get; set; } = 0;

        public static int SyncOffset { get; set; } = 0;

        public static int CurrentMode = 0;

        public static int SyncDivider { get; set; } = 8;

        public static int CFDZeroCross0 { get; set; } = 10;
        public static int CFDLevel0 { get; set; } = 50;

        public static int CFDZeroCross1 { get; set; } = 10;
        public static int CFDLevel1 { get; set; } = 50;
    }
}
