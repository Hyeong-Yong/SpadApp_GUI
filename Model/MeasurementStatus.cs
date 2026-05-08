using System;
using System.Collections.Generic;
using System.Text;

namespace SpadApp.Model
{
    public static class MeasurementStatus
    {
        public static double ResolutionPs { get; set; }

        public static int AcquisitionTimeMs { get; set; }

        public static int CountRate0 { get; set; }

        public static int CountRate1 { get; set; }

        public static int Flags { get; set; }

        public static int CTCStatus { get; set; }

        public static bool Overflow =>
            (Flags & 0x0040) != 0;

        public static int IsMeasuring { get; set; }
    }
}
