namespace IrrigationScada.Web;

/// <summary>
/// Опис датчика на мнемосхемі: назва, MQTT-тема, одиниця вимірювання та позиція.
/// </summary>
public class SensorView
{
    public string Name { get; set; } = "";
    public string Topic { get; set; } = "";
    public string Unit { get; set; } = "";
    public int X { get; set; }   // позиція на мнемосхемі, px
    public int Y { get; set; }
}
