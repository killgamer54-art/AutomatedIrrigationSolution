using System.Collections.Concurrent;
using IrrigationScada.Components;

namespace IrrigationScada.Web.Services;

/// <summary>
/// Кеш оперативного стану: останнє отримане показання для кожної MQTT-теми.
/// </summary>
public class LiveValuesCache
{
    private readonly ConcurrentDictionary<string, SensorReading> _values = new();

    public void Update(SensorReading reading) => _values[reading.Topic] = reading;

    public IReadOnlyList<SensorReading> GetAll() =>
        _values.Values.OrderBy(r => r.Topic).ToList();
}
