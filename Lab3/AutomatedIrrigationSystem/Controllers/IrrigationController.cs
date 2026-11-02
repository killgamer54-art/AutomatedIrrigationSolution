using System;

namespace AutomatedIrrigationSystem
{
    // --- ГОЛОВНИЙ КЛАС СИСТЕМИ УПРАВЛІННЯ (КОНТРОЛЕР ЗРОШЕННЯ) ---
    public class IrrigationController
    {
        public SoilMoistureSensor MoistureSensor { get; set; }
        public PressureSensor PressureSensor { get; set; }
        public IrrigationValve ZoneValve { get; set; }
        public WaterPump MainPump { get; set; }

        public double MinMoistureThreshold { get; set; } = 30.0;    // Нижній поріг (%)
        public double TargetMoistureThreshold { get; set; } = 70.0; // Оптимальний рівень (%)

        public IrrigationController(SoilMoistureSensor sensor, IrrigationValve valve, WaterPump pump)
        {
            MoistureSensor = sensor;
            ZoneValve = valve;
            MainPump = pump;
            PressureSensor = new PressureSensor();
        }

        // Логіка автоматичного керування
        public void ProcessAutomationCycle()
        {
            double currentMoisture = MoistureSensor.ReadMoisture();
            Console.WriteLine($"--- Цикл контролю: Зона {MoistureSensor.ZoneName} | Вологість: {currentMoisture:F1}% ---");

            if (currentMoisture < MinMoistureThreshold)
            {
                Console.WriteLine("Попередження: Вологість нижча норми! Запуск зрошення...");
                if (MainPump.State == PumpState.Off) MainPump.Start();
                if (ZoneValve.State == ValveState.Closed) ZoneValve.Open();
            }
            else if (currentMoisture >= TargetMoistureThreshold)
            {
                Console.WriteLine("Норма досягнута. Зупинка зрошення...");
                if (ZoneValve.State == ValveState.Open) ZoneValve.Close();
                if (MainPump.State == PumpState.Running) MainPump.Stop();
            }
            else
            {
                Console.WriteLine("Стан системи в нормі. Змін не потрібно.");
            }
        }
    }
}
