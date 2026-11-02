using System.Text;
using System.Text.Json;
using IrrigationDomainLibrary.Controllers;
using MQTTnet;
using MQTTnet.Client;

// Лабораторна робота №5. Варіант №9 — система зрошення сільськогосподарських угідь.
// Консольний застосунок використовує компоненти з бібліотеки IrrigationDomainLibrary
// (підключена через <ProjectReference>) і публікує телеметрію у форматі JSON через MQTT.

Console.OutputEncoding = Encoding.UTF8;

string broker = args.Length > 0 ? args[0] : "broker.hivemq.com";
int port = args.Length > 1 && int.TryParse(args[1], out var p) ? p : 1883;
const string baseTopic = "ikm/irrigation/variant9";

var controller = IrrigationController.CreateDefault(sectors: 4);
var jsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

var factory = new MqttFactory();
using var mqttClient = factory.CreateMqttClient();
var mqttOptions = new MqttClientOptionsBuilder()
    .WithTcpServer(broker, port)
    .WithClientId($"irrigation-{Guid.NewGuid():N}")
    .WithCleanSession()
    .Build();

bool online = false;
try
{
    await mqttClient.ConnectAsync(mqttOptions, CancellationToken.None);
    online = true;
    Console.WriteLine($"Підключено до MQTT-брокера {broker}:{port}. Топік: {baseTopic}/telemetry");
}
catch (Exception ex)
{
    Console.WriteLine($"Не вдалося підключитися до брокера ({ex.Message}). Робота в автономному режимі.");
}

using var cts = new CancellationTokenSource();
Console.CancelKeyPress += (_, e) => { e.Cancel = true; cts.Cancel(); };
Console.WriteLine("Натисніть Ctrl+C для завершення.\n");

while (!cts.IsCancellationRequested)
{
    var snapshot = controller.ScanCycle();
    string json = JsonSerializer.Serialize(snapshot, jsonOptions);

    Console.WriteLine($"[Цикл {snapshot.Cycle}] Тиск {snapshot.LinePressure:F2} бар | Насос: {(snapshot.PumpRunning ? "ON" : "OFF")}");
    foreach (var s in snapshot.Sectors)
        Console.WriteLine($"   Сектор {s.Sector}: вологість {s.Moisture,5:F1} %  клапан {(s.ValveOpen ? "ВІДКРИТО" : "закрито")}");
    foreach (var alarm in snapshot.Alarms)
        Console.WriteLine($"   ! {alarm}");

    if (online && mqttClient.IsConnected)
    {
        var message = new MqttApplicationMessageBuilder()
            .WithTopic($"{baseTopic}/telemetry")
            .WithPayload(json)
            .WithQualityOfServiceLevel(MQTTnet.Protocol.MqttQualityOfServiceLevel.AtLeastOnce)
            .Build();
        await mqttClient.PublishAsync(message, CancellationToken.None);
    }

    try { await Task.Delay(controller.CycleTime, cts.Token); }
    catch (TaskCanceledException) { break; }
}

if (mqttClient.IsConnected)
    await mqttClient.DisconnectAsync();

Console.WriteLine("Роботу завершено.");
