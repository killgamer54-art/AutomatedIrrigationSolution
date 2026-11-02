namespace IrrigationDomainLibrary.Sensors;

/// <summary>
/// Датчик тиску в магістральному трубопроводі (бар).
/// </summary>
public class PressureSensor
{
    private readonly Random _random;

    public string Id { get; }
    public double Pressure { get; private set; }
    public double MinAllowed { get; }
    public double MaxAllowed { get; }

    public bool IsLow => Pressure < MinAllowed;
    public bool IsHigh => Pressure > MaxAllowed;

    public PressureSensor(string id, double minAllowed = 1.5, double maxAllowed = 4.5, int? seed = null)
    {
        Id = id;
        MinAllowed = minAllowed;
        MaxAllowed = maxAllowed;
        _random = seed.HasValue ? new Random(seed.Value) : new Random();
    }

    /// <summary>
    /// Симулює тиск: залежить від того, чи працює насос і скільки клапанів відкрито.
    /// </summary>
    public void Simulate(bool pumpRunning, int openValves)
    {
        double target = pumpRunning ? 4.0 - 0.4 * openValves : 0.2;
        double noise = (_random.NextDouble() - 0.5) * 0.2;
        Pressure = Math.Round(Math.Max(0, Pressure + (target - Pressure) * 0.6 + noise), 2);
    }

    public override string ToString() => $"{Id}: {Pressure:F2} бар";
}
