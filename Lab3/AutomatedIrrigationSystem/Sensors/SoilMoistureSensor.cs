using System;

namespace AutomatedIrrigationSystem
{
    // --- ДАТЧИК ВОЛОГОСТІ ҐРУНТУ ---
    public class SoilMoistureSensor : Sensor
    {
        public string ZoneName { get; private set; }
        public double CurrentMoisture => Value; // % вологості ґрунту

        public SoilMoistureSensor(string sensorId, string zoneName)
            : base(sensorId, "Вологість ґрунту", "%", 45.0) // 45 % — значення за замовчуванням
        {
            ZoneName = zoneName;
        }

        public double ReadMoisture()
        {
            // Імітація зчитування даних з датчика
            return Read();
        }

        public void SimulateMoistureChange(double delta)
        {
            Value = Math.Min(100.0, Math.Max(0.0, Value + delta));
        }
    }
}
