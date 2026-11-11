namespace AutomatedIrrigationSystem
{
    // --- ДАТЧИК ТЕМПЕРАТУРИ ПОВІТРЯ ---
    // Використовується для адаптивного коригування поливу в спеку.
    public class AirTemperatureSensor : Sensor
    {
        public double TemperatureC => Value;

        public AirTemperatureSensor(string sensorId)
            : base(sensorId, "Температура повітря", "°C", 22.0)
        {
        }

        public double ReadTemperature()
        {
            return Read();
        }

        public void SetTemperature(double temperature)
        {
            Value = temperature;
        }
    }
}
