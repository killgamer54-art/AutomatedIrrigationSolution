using IrrigationDomainLibrary.Actuators;
using IrrigationDomainLibrary.Sensors;

namespace IrrigationDomainLibrary.Controllers;

/// <summary>
/// Знімок стану системи після одного циклу сканування (для телеметрії).
/// </summary>
public record SectorState(int Sector, double Moisture, bool ValveOpen);

public record IrrigationSnapshot(
    DateTime Timestamp,
    long Cycle,
    double LinePressure,
    bool PumpRunning,
    bool PumpFault,
    IReadOnlyList<SectorState> Sectors,
    IReadOnlyList<string> Alarms);

/// <summary>
/// Контролер автоматичного підтримання вологості ґрунту.
/// Логіка з гістерезисом: клапан відкривається при вологості нижче нижньої межі
/// і закривається при досягненні верхньої. Насос працює, поки відкрито хоча б один клапан.
/// </summary>
public class IrrigationController
{
    private readonly List<SoilMoistureSensor> _moistureSensors;
    private readonly List<IrrigationValve> _valves;
    private long _cycle;

    public PressureSensor PressureSensor { get; }
    public WaterPump Pump { get; }
    public double LowerThreshold { get; set; }
    public double UpperThreshold { get; set; }
    public TimeSpan CycleTime { get; }

    public IReadOnlyList<SoilMoistureSensor> MoistureSensors => _moistureSensors;
    public IReadOnlyList<IrrigationValve> Valves => _valves;

    public IrrigationController(
        IEnumerable<SoilMoistureSensor> moistureSensors,
        IEnumerable<IrrigationValve> valves,
        PressureSensor pressureSensor,
        WaterPump pump,
        double lowerThreshold = 35,
        double upperThreshold = 60,
        TimeSpan? cycleTime = null)
    {
        _moistureSensors = moistureSensors.ToList();
        _valves = valves.ToList();
        PressureSensor = pressureSensor;
        Pump = pump;
        LowerThreshold = lowerThreshold;
        UpperThreshold = upperThreshold;
        CycleTime = cycleTime ?? TimeSpan.FromSeconds(1);

        if (LowerThreshold >= UpperThreshold)
            throw new ArgumentException("Нижня межа вологості має бути меншою за верхню.");
    }

    /// <summary>Створює типову конфігурацію на задану кількість секторів.</summary>
    public static IrrigationController CreateDefault(int sectors = 4, int? seed = null)
    {
        var sensors = Enumerable.Range(1, sectors)
            .Select(i => new SoilMoistureSensor($"SM-{i:00}", i, 30 + i * 8, seed + i));
        var valves = Enumerable.Range(1, sectors)
            .Select(i => new IrrigationValve($"V-{i:00}", i));
        return new IrrigationController(sensors, valves, new PressureSensor("PT-01", seed: seed), new WaterPump("P-01"));
    }

    /// <summary>Виконує один цикл сканування: читання → логіка → вихід → симуляція.</summary>
    public IrrigationSnapshot ScanCycle()
    {
        _cycle++;
        var alarms = new List<string>();

        // 1. Логіка керування клапанами (гістерезис)
        foreach (var sensor in _moistureSensors)
        {
            var valve = _valves.FirstOrDefault(v => v.SectorNumber == sensor.SectorNumber);
            if (valve is null) continue;

            if (sensor.Moisture < LowerThreshold) valve.Open();
            else if (sensor.Moisture >= UpperThreshold) valve.Close();
        }

        // 2. Захист за тиском
        if (PressureSensor.IsHigh)
        {
            Pump.Trip();
            alarms.Add($"Високий тиск у магістралі: {PressureSensor.Pressure:F2} бар — насос аварійно зупинено");
        }

        // 3. Керування насосом
        int openValves = _valves.Count(v => v.IsOpen);
        if (openValves > 0) Pump.Start();
        else Pump.Stop();

        if (Pump.IsRunning && _cycle > 2 && PressureSensor.IsLow)
            alarms.Add($"Низький тиск у магістралі: {PressureSensor.Pressure:F2} бар");
        if (Pump.IsFault)
            alarms.Add("Насос в аварійному стані");

        // 4. Симуляція фізичного процесу
        bool waterAvailable = Pump.IsRunning;
        foreach (var sensor in _moistureSensors)
        {
            bool irrigating = waterAvailable &&
                              _valves.Any(v => v.SectorNumber == sensor.SectorNumber && v.IsOpen);
            sensor.Simulate(irrigating);
        }
        PressureSensor.Simulate(Pump.IsRunning, openValves);
        Pump.Tick(CycleTime);

        return CreateSnapshot(alarms);
    }

    private IrrigationSnapshot CreateSnapshot(List<string> alarms)
    {
        var sectors = _moistureSensors
            .Select(s => new SectorState(
                s.SectorNumber,
                s.Moisture,
                _valves.Any(v => v.SectorNumber == s.SectorNumber && v.IsOpen)))
            .ToList();

        return new IrrigationSnapshot(
            DateTime.UtcNow, _cycle, PressureSensor.Pressure,
            Pump.IsRunning, Pump.IsFault, sectors, alarms);
    }
}
