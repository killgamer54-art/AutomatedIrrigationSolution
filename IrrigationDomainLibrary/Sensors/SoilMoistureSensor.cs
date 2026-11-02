namespace IrrigationDomainLibrary.Sensors;

/// <summary>
/// Датчик вологості ґрунту (%) для одного сектора поля.
/// Підтримує симуляцію: без поливу ґрунт висихає, під час поливу — зволожується.
/// </summary>
public class SoilMoistureSensor
{
    private readonly Random _random;

    public string Id { get; }
    public int SectorNumber { get; }
    public double Moisture { get; private set; }

    public SoilMoistureSensor(string id, int sectorNumber, double initialMoisture, int? seed = null)
    {
        Id = id;
        SectorNumber = sectorNumber;
        Moisture = Math.Clamp(initialMoisture, 0, 100);
        _random = seed.HasValue ? new Random(seed.Value) : new Random();
    }

    /// <summary>Симулює зміну вологості за один цикл сканування.</summary>
    /// <param name="isIrrigating">Чи відкрито клапан сектора.</param>
    public void Simulate(bool isIrrigating)
    {
        double delta = isIrrigating
            ? 2.5 + _random.NextDouble() * 1.5      // полив: +2.5..4.0 %
            : -(0.5 + _random.NextDouble() * 1.0);  // випаровування: -0.5..1.5 %
        Moisture = Math.Round(Math.Clamp(Moisture + delta, 0, 100), 1);
    }

    public override string ToString() => $"{Id} (сектор {SectorNumber}): {Moisture:F1} %";
}
