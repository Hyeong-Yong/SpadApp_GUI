namespace SpadApp.Parameters
{
    /// <summary>
    /// PM100 실시간 측정 상태
    /// </summary>
    public class PM100_MeasurementStatus
    {
        // ------------------------------------------------------------
        // Measurement
        // ------------------------------------------------------------

        public double CurrentWavelength { get; set; }
            = 780;

        public double CurrentPower { get; set; }

        public string LastUpdateTime { get; set; }
            = string.Empty;
    }
}