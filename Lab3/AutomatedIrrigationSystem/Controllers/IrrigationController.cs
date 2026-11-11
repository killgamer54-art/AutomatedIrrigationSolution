using System;

namespace AutomatedIrrigationSystem
{
    // --- ГОЛОВНИЙ КЛАС СИСТЕМИ УПРАВЛІННЯ (КОНТРОЛЕР ЗРОШЕННЯ) ---
    public class IrrigationController
    {
        // Сенсори
        public SoilMoistureSensor MoistureSensor { get; set; }
        public PressureSensor PressureSensor { get; set; }
        public AirTemperatureSensor TemperatureSensor { get; set; }
        public RainSensor RainSensor { get; set; }

        // Виконавчі механізми
        public IrrigationValve ZoneValve { get; set; }
        public WaterPump MainPump { get; set; }
        public FertigationSystem Fertigation { get; set; }

        // Уставки
        public double MinMoistureThreshold { get; set; } = 30.0;    // Нижній поріг (%)
        public double TargetMoistureThreshold { get; set; } = 70.0; // Оптимальний рівень (%)
        public double HotDayTemperature { get; set; } = 30.0;       // °C, від цієї температури — спека
        public double HotDayMoistureBonus { get; set; } = 10.0;     // на скільки % піднімається нижній поріг у спеку
        public double MinPressureBar { get; set; } = 1.5;           // допустимий тиск під час роботи насоса
        public double MaxPressureBar { get; set; } = 6.0;
        public bool FertigationEnabled { get; set; } = true;

        public bool IsIrrigating => MainPump.State == PumpState.Running && ZoneValve.State == ValveState.Open;

        public IrrigationController(SoilMoistureSensor moistureSensor, PressureSensor pressureSensor,
            AirTemperatureSensor temperatureSensor, RainSensor rainSensor,
            IrrigationValve valve, WaterPump pump, FertigationSystem fertigation)
        {
            MoistureSensor = moistureSensor;
            PressureSensor = pressureSensor;
            TemperatureSensor = temperatureSensor;
            RainSensor = rainSensor;
            ZoneValve = valve;
            MainPump = pump;
            Fertigation = fertigation;
        }

        // Логіка автоматичного керування (один цикл сканування)
        public void ProcessAutomationCycle()
        {
            double moisture = MoistureSensor.ReadMoisture();
            double temperature = TemperatureSensor.ReadTemperature();
            double rain = RainSensor.ReadRainIntensity();
            double pressure = PressureSensor.ReadPressure();

            Console.WriteLine($"--- Цикл контролю: Зона {MoistureSensor.ZoneName} | Вологість: {moisture:F1}% | " +
                              $"T: {temperature:F1} °C | Опади: {rain:F1} мм/год | Тиск: {pressure:F1} бар ---");

            // 1. Захист насоса: насос в аварії — керування заблоковано до скидання
            if (MainPump.State == PumpState.Fault)
            {
                Console.WriteLine("Насос в аварійному стані. Потрібне втручання оператора.");
                return;
            }

            // 2. Контроль тиску під час роботи насоса
            if (MainPump.State == PumpState.Running && (pressure < MinPressureBar || pressure > MaxPressureBar))
            {
                string reason = pressure > MaxPressureBar
                    ? $"тиск {pressure:F1} бар > {MaxPressureBar:F1} бар (можливе засмічення магістралі)"
                    : $"тиск {pressure:F1} бар < {MinPressureBar:F1} бар (можливий прорив магістралі)";
                StopIrrigation(emergency: true);
                MainPump.EmergencyStop(reason);
                return;
            }

            // 3. Під час дощу полив не потрібен
            if (RainSensor.IsRaining)
            {
                Console.WriteLine("Йде дощ — полив відкладено.");
                if (IsIrrigating) StopIrrigation();
                return;
            }

            // 4. Адаптація до погоди: у спеку нижній поріг вологості підвищується
            double minThreshold = MinMoistureThreshold;
            if (temperature >= HotDayTemperature)
            {
                minThreshold += HotDayMoistureBonus;
                Console.WriteLine($"Спека: нижній поріг вологості підвищено до {minThreshold:F0}%.");
            }

            // 5. Двопозиційне регулювання вологості ґрунту
            if (moisture < minThreshold)
            {
                if (IsIrrigating)
                {
                    Console.WriteLine("Полив триває.");
                    return;
                }
                Console.WriteLine("Попередження: Вологість нижча норми! Запуск зрошення...");
                StartIrrigation();
            }
            else if (moisture >= TargetMoistureThreshold)
            {
                if (IsIrrigating)
                {
                    Console.WriteLine("Норма досягнута. Зупинка зрошення...");
                    StopIrrigation();
                }
                else
                {
                    Console.WriteLine("Вологість у нормі. Полив не потрібен.");
                }
            }
            else
            {
                Console.WriteLine(IsIrrigating ? "Полив триває." : "Стан системи в нормі. Змін не потрібно.");
            }
        }

        private void StartIrrigation()
        {
            if (MainPump.State == PumpState.Off) MainPump.Start();
            if (ZoneValve.State == ValveState.Closed) ZoneValve.Open();
            if (FertigationEnabled && Fertigation.State == FertigationState.Idle) Fertigation.StartDosing();
        }

        private void StopIrrigation(bool emergency = false)
        {
            if (Fertigation.State == FertigationState.Dosing) Fertigation.StopDosing();
            if (ZoneValve.State == ValveState.Open) ZoneValve.Close();
            if (!emergency && MainPump.State == PumpState.Running) MainPump.Stop();
        }
    }
}
