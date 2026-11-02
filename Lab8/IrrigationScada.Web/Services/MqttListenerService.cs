using System.Text.Json;
using IrrigationScada.Components;
using MQTTnet;
using MQTTnet.Client;

namespace IrrigationScada.Web.Services;

/// <summary>
/// Фонова служба: підписується на теми irrigation/9/sensors/# і оновлює LiveValuesCache.
/// </summary>
public class MqttListenerService : BackgroundService
{
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    private readonly LiveValuesCache _cache;
    private readonly IConfiguration _config;
    private readonly ILogger<MqttListenerService> _logger;

    public MqttListenerService(LiveValuesCache cache, IConfiguration config, ILogger<MqttListenerService> logger)
    {
        _cache = cache;
        _config = config;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var factory = new MqttFactory();
        using var client = factory.CreateMqttClient();

        client.ApplicationMessageReceivedAsync += e =>
        {
            try
            {
                var payload = JsonSerializer.Deserialize<SensorPayload>(
                    e.ApplicationMessage.ConvertPayloadToString(), JsonOptions);
                if (payload is not null)
                {
                    _cache.Update(new SensorReading(
                        e.ApplicationMessage.Topic, payload.Value, payload.Unit, payload.MeasuredAtUtc));
                }
            }
            catch (JsonException ex)
            {
                _logger.LogWarning(ex, "Некоректне повідомлення в темі {Topic}", e.ApplicationMessage.Topic);
            }
            return Task.CompletedTask;
        };

        var options = new MqttClientOptionsBuilder()
            .WithTcpServer(_config["Mqtt:Host"] ?? "127.0.0.1", int.Parse(_config["Mqtt:Port"] ?? "1883"))
            .WithClientId($"irrigation-scada-{Guid.NewGuid():N}")
            .Build();

        var subscribeOptions = factory.CreateSubscribeOptionsBuilder()
            .WithTopicFilter(f => f.WithTopic(SensorTopics.All))
            .Build();

        // Підключення з повторними спробами, поки застосунок працює
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                if (!client.IsConnected)
                {
                    await client.ConnectAsync(options, stoppingToken);
                    await client.SubscribeAsync(subscribeOptions, stoppingToken);
                    _logger.LogInformation("Підписано на {Topic}", SensorTopics.All);
                }
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                _logger.LogWarning("MQTT-брокер недоступний: {Message}", ex.Message);
            }

            try { await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken); }
            catch (OperationCanceledException) { break; }
        }
    }
}
