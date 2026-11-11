namespace AutomatedIrrigationSystem
{
    // --- ДАТЧИК ТИСКУ В МАГІСТРАЛІ ---
    public class PressureSensor : Sensor
    {
        public double PressureBar => Value;

        public PressureSensor(string sensorId)
            : base(sensorId, "Тиск у магістралі", "бар", 0.0)
        {
        }

        public double ReadPressure()
        {
            return Read();
        }

        public void SetPressure(double pressure)
        {
            Value = pressure;
        }
    }
}
