using System;
using System.Text;

namespace AutomatedIrrigationSystem
{
    // --- ДЕМОНСТРАЦІЯ РОБОТИ ---
    class Program
    {
        static void Main()
        {
            Console.OutputEncoding = Encoding.UTF8;

            var sensor = new SoilMoistureSensor("S-01", "Сектор 1 (Пшениця)");
            var valve = new IrrigationValve("Z-01");
            var pump = new WaterPump("PUMP-01");
            var controller = new IrrigationController(sensor, valve, pump);

            // Крок 1: Нормальний стан
            controller.ProcessAutomationCycle();

            // Крок 2: Імітація висихання ґрунту
            sensor.SimulateMoistureChange(-20.0); // Вологість стає 25%
            controller.ProcessAutomationCycle();

            // Крок 3: Імітація наповнення ґрунту вологою під час поливу
            sensor.SimulateMoistureChange(50.0); // Вологість стає 75%
            controller.ProcessAutomationCycle();
        }
    }
}
