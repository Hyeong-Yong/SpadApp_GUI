using System;
using System.Collections.Generic;
using System.Text;

namespace SpadApp.Model
{
    public class PhotonStatistics
    {
        public double PhotonEnergy { get; set; }

        public double InputPowerW { get; set; }

        public double AttenuatedPowerW { get; set; }

        public double PhotonFlux { get; set; }

        public double MeanPerPulse { get; set; }

        public double PDE { get; set; }

        public double RepetitionRate { get; set; }

        public double DetectedCountRate { get; set; }

        public double AttenuationDb { get; set; }
    }
}
