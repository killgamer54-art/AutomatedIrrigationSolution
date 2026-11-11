namespace AutomatedIrrigationSystem
{
    // --- ДАТЧИК ОПАДІВ ---
    // Вимірює інтенсивність опадів; під час дощу полив відкладається.
    public class RainSensor : Sensor
    {
        public double RainThreshold { get; set; } = 0.5; // мм/год, вище — вважаємо, що йде дощ

        public double IntensityMmPerHour => Value;
        public bool IsRaining => Read() > RainThreshold;

        public RainSensor(string sensorId)
            : base(sensorId, "Опади", "мм/год", 0.0)
        {
        }

        public double ReadRainIntensity()
        {
            return Read();
        }

        public void SetRainIntensity(double intensity)
        {
            Value = intensity;
        }
    }
}
