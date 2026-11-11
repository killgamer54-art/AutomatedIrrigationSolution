using IrrigationScada.Components;

namespace IrrigationScada.Web.Services;

/// <summary>
/// Конфігурація мнемосхеми системи зрошення: розташування датчиків і аварійні пороги.
/// </summary>
public class SchemeConfigService
{
    private readonly List<SensorView> _sensors = new()
    {
        new() { Name = "Тиск у магістралі",   Topic = SensorTopics.Pressure,     Unit = "bar",  X = 150, Y = 40,  Max = 5 },
        new() { Name = "Витрата води",        Topic = SensorTopics.FlowRate,     Unit = "m3/h", X = 360, Y = 40,  Max = 20 },
        new() { Name = "Вологість ґрунту",    Topic = SensorTopics.SoilMoisture, Unit = "%",    X = 560, Y = 230, Max = 80 },
        new() { Name = "Температура повітря", Topic = SensorTopics.Temperature,  Unit = "C",    X = 560, Y = 40,  Max = 28 }
    };

    public IReadOnlyList<SensorView> GetSensors() => _sensors;
}
