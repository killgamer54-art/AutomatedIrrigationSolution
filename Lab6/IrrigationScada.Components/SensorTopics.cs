namespace IrrigationScada.Components;

/// <summary>
/// MQTT-теми датчиків системи зрошення (Варіант №9).
/// Структура: &lt;object&gt;/&lt;id&gt;/sensors/&lt;sensor-name&gt;
/// </summary>
public static class SensorTopics
{
    public const string Root = "irrigation/9/sensors";
    public const string All = Root + "/#";

    public const string SoilMoisture = Root + "/soil_moisture";
    public const string Pressure = Root + "/pressure";
    public const string FlowRate = Root + "/flow_rate";
    public const string Temperature = Root + "/temperature";
}
