using System;
using System.Globalization;
using System.Text;

namespace AutomatedIrrigationSystem
{
    // --- ДЕМОНСТРАЦІЯ РОБОТИ ---
    class Program
    {
        static void Main()
        {
            Console.OutputEncoding = Encoding.UTF8;
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture; // десяткова крапка у виводі

            // Сенсори
            var moisture = new SoilMoistureSensor("S-01", "Сектор 1 (Пшениця)");
            var pressure = new PressureSensor("P-01");
            var temperature = new AirTemperatureSensor("T-01");
            var rain = new RainSensor("R-01");

            // Виконавчі механізми
            var valve = new IrrigationValve("Z-01");
            var pump = new WaterPump("PUMP-01");
            var fertigation = new FertigationSystem("F-01", 12.5);

            var controller = new IrrigationController(moisture, pressure, temperature, rain, valve, pump, fertigation);

            // Крок 1: Нормальний стан
            controller.ProcessAutomationCycle();

            // Крок 2: Імітація висихання ґрунту — вологість стає 25%
            moisture.SimulateMoistureChange(-20.0);
            controller.ProcessAutomationCycle();
            pressure.SetPressure(3.2); // насос вийшов на робочий тиск

            // Крок 3: Ґрунт наповнився вологою під час поливу — вологість стає 75%
            moisture.SimulateMoistureChange(50.0);
            controller.ProcessAutomationCycle();
            pressure.SetPressure(0.0);

            // Крок 4: Спекотний день — вологість 35% нижча за підвищений поріг 40%
            temperature.SetTemperature(33.0);
            moisture.SimulateMoistureChange(-40.0);
            controller.ProcessAutomationCycle();
            pressure.SetPressure(3.0);

            // Крок 5: Почався дощ — полив зупиняється
            rain.SetRainIntensity(4.0);
            controller.ProcessAutomationCycle();
            pressure.SetPressure(0.0);

            // Крок 6: Дощ закінчився — полив відновлюється
            rain.SetRainIntensity(0.0);
            controller.ProcessAutomationCycle();

            // Крок 7: Засмічення магістралі — тиск зростає до 7.5 бар
            pressure.SetPressure(7.5);
            controller.ProcessAutomationCycle();

            // Крок 8: Насос в аварії — керування заблоковане
            controller.ProcessAutomationCycle();

            // Перелік сенсорів через спільний базовий клас
            Console.WriteLine();
            Console.WriteLine("Стан сенсорів:");
            Sensor[] sensors = { moisture, pressure, temperature, rain };
            foreach (Sensor s in sensors)
            {
                Console.WriteLine("  " + s);
            }
        }
    }
}
