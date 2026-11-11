namespace IrrigationScada.Components;

/// <summary>
/// Одне показання датчика, отримане з MQTT-теми.
/// </summary>
public record SensorReading(string Topic, double Value, string Unit, DateTimeOffset MeasuredAtUtc);

/// <summary>
/// Формат JSON-повідомлення, яке симулятор публікує в MQTT:
/// {"value":45.5,"unit":"%","measuredAtUtc":"2026-10-22T14:00:00+00:00"}
/// </summary>
public record SensorPayload(double Value, string Unit, DateTimeOffset MeasuredAtUtc);
