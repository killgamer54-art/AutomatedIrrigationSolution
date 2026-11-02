using System;

namespace AutomatedIrrigationSystem
{
    // --- БАЗОВИЙ КЛАС СЕНСОРА: ДАТЧИК ВОЛОГОСТІ ҐРУНТУ ---
    public class SoilMoistureSensor
    {
        public string SensorId { get; private set; }
        public string ZoneName { get; private set; }
        public double CurrentMoisture { get; private set; } // % вологості ґрунту

        public SoilMoistureSensor(string sensorId, string zoneName)
        {
            SensorId = sensorId;
            ZoneName = zoneName;
            CurrentMoisture = 45.0; // Значення за замовчуванням
        }

        public double ReadMoisture()
        {
            // Імітація зчитування даних з датчика
            return CurrentMoisture;
        }

        public void SimulateMoistureChange(double delta)
        {
            CurrentMoisture = Math.Min(100.0, Math.Max(0.0, CurrentMoisture + delta));
        }
    }
}
