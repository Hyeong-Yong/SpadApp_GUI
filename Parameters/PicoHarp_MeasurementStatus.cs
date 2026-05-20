using System;
using System.Collections.Generic;
using System.Text;

namespace SpadApp.Parameters
{
    public static class PicoHarp_MeasurementStatus
    {
        public static double ResolutionPs { get; set; }

        public static int AcquisitionTimeMs { get; set; } = 1000;

        public static int AcqOffset { get; set; } = 0;

        public static int CountRate0 { get; set; }

        public static int CountRate1 { get; set; }

        public static int Flags { get; set; }

        public static int CTCStatus { get; set; }

        public static bool Overflow =>
            (Flags & 0x0040) != 0;

        public static int IsMeasuring { get; set; }
    }
}
