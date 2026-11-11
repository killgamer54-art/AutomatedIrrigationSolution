namespace IrrigationScada.Web;

/// <summary>
/// Опис датчика на мнемосхемі: назва, MQTT-тема, позиція та аварійний поріг.
/// </summary>
public class SensorView
{
    public string Name { get; set; } = "";
    public string Topic { get; set; } = "";
    public string Unit { get; set; } = "";
    public int X { get; set; }   // позиція на мнемосхемі, px
    public int Y { get; set; }
    public int Max { get; set; } // аварійний поріг: значення > Max підсвічується червоним
}
