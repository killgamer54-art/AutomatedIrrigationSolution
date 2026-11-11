using System.Text;
using System.Text.Json;
using IrrigationScada.Components;
using MQTTnet;
using MQTTnet.Client;

// Симулятор технологічного об'єкта: система зрошення с/г угідь (Варіант №9).
// Кожні 2 секунди публікує показання датчиків у MQTT-брокер Mosquitto (127.0.0.1:1883).

Console.OutputEncoding = Encoding.UTF8;

var random = new Random();
var sensors = new List<SimSensor>
{
    new(SensorTopics.SoilMoisture, "%",    45.0, 20, 90,  2.0),
    new(SensorTopics.Pressure,     "bar",   4.0,  0,  6,  0.2),
    new(SensorTopics.FlowRate,     "m3/h", 12.5,  0, 25,  1.0),
    new(SensorTopics.Temperature,  "C",    26.0, 15, 35,  0.8)
};

var factory = new MqttFactory();
using var client = factory.CreateMqttClient();
var options = new MqttClientOptionsBuilder()
    .WithTcpServer("127.0.0.1", 1883)
    .WithClientId("irrigation-simulator")
    .Build();

await client.ConnectAsync(options);
Console.WriteLine("Симулятор підключено до MQTT-брокера. Ctrl+C — завершення.");

using var cts = new CancellationTokenSource();
Console.CancelKeyPress += (_, e) => { e.Cancel = true; cts.Cancel(); };

while (!cts.IsCancellationRequested)
{
    foreach (var s in sensors)
    {
        s.Step(random);
        string payload = JsonSerializer.Serialize(new
        {
            value = Math.Round(s.Value, 2),
            unit = s.Unit,
            measuredAtUtc = DateTimeOffset.UtcNow
        });

        var message = new MqttApplicationMessageBuilder()
            .WithTopic(s.Topic)
            .WithPayload(payload)
            .Build();
        await client.PublishAsync(message, CancellationToken.None);
        Console.WriteLine($"{s.Topic} {payload}");
    }

    try { await Task.Delay(TimeSpan.FromSeconds(2), cts.Token); }
    catch (TaskCanceledException) { break; }
}

await client.DisconnectAsync();

/// <summary>Датчик із випадковим блуканням значення в заданих межах.</summary>
class SimSensor(string topic, string unit, double initial, double min, double max, double step)
{
    public string Topic { get; } = topic;
    public string Unit { get; } = unit;
    public double Value { get; private set; } = initial;

    public void Step(Random random)
    {
        Value = Math.Clamp(Value + (random.NextDouble() * 2 - 1) * step, min, max);
    }
}
