using SpadApp.Model;

namespace SpadApp.Utility
{
    public class PhotonCalculator
    {
        // ------------------------------------------------------------
        // Constants
        // ------------------------------------------------------------
        private const double planckConstant = 6.626e-34;
        private const double lightSpeed = 3.0e8;
        private const double NmToMeter = 1e-9;           // nm -> m 변환 계수
        // ------------------------------------------------------------
        // Main Calculation
        // ------------------------------------------------------------

        public static PhotonStatistics Calculate(
            double laserPowerW,
            double repetitionRate,
            double detectedCountRate,
            double attenuationDb, double wavelength)
        {
            PhotonStatistics result = new PhotonStatistics();

            double waveLengthMeter = wavelength * NmToMeter;

            double photonEnergy = planckConstant * lightSpeed /wavelength;

            double attenuatedPower = laserPowerW * Math.Pow(10.0, -attenuationDb / 10.0);

            double photonFlux =
                attenuatedPower /
                photonEnergy;

            double meanPerPulse = 0;

            if (repetitionRate > 0)
            {
                meanPerPulse =
                    photonFlux /
                    repetitionRate;
            }

            double pde = 0;

            if (photonFlux > 0)
            {
                pde =
                    detectedCountRate /
                    photonFlux;
            }

            result.PhotonEnergy = photonEnergy;
            result.InputPowerW = laserPowerW;
            result.AttenuatedPowerW = attenuatedPower;
            result.PhotonFlux = photonFlux;
            result.MeanPerPulse = meanPerPulse;
            result.PDE = pde;
            result.RepetitionRate = repetitionRate;
            result.DetectedCountRate = detectedCountRate;
            result.AttenuationDb = attenuationDb;

            return result;
        }
    }
}