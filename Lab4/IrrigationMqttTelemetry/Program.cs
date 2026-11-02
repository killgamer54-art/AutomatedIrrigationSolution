using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using MQTTnet;

namespace AutomatedIrrigationSystem
{
    class Program
    {
        private record SensorConfiguration(
            string Name,
            Func<double> ReadValueFunc,
            string Topic,
            string Unit
        );

        static async Task Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;

            // 1. Ініціалізація MQTT клієнта (Mosquitto на 127.0.0.1:1883)
            var factory = new MqttClientFactory();
            using var mqttClient = factory.CreateMqttClient();

            var options = new MqttClientOptionsBuilder()
                .WithTcpServer("127.0.0.1", 1883)
                .Build();

            await mqttClient.ConnectAsync(options);
            Console.WriteLine("Підключено до MQTT-брокера Mosquitto!");

            // 2. Конфігурація сенсорів відповідно до Варіанта 9
            var sensors = new List<SensorConfiguration>
            {
                new("Вологість ґрунту",    () => 45.5, "irrigation/9/sensors/soil_moisture", "%"),
                new("Тиск у магістралі",   () => 4.2,  "irrigation/9/sensors/pressure",      "bar"),
                new("Витрата води",        () => 12.5, "irrigation/9/sensors/flow_rate",     "m3/h"),
                new("Температура повітря", () => 24.8, "irrigation/9/sensors/temperature",   "C")
            };

            // 3. Цикл симуляції: 10 ітерацій із затримкою 2 секунди
            for (int iteration = 1; iteration <= 10; iteration++)
            {
                Console.WriteLine($"\n--- Цикл передачі телеметрії №{iteration} ---");

                foreach (var sc in sensors)
                {
                    double val = sc.ReadValueFunc();

                    // Формування JSON payload
                    string payload = JsonSerializer.Serialize(new
                    {
                        value = Math.Round(val, 2),
                        unit = sc.Unit,
                        measuredAtUtc = DateTimeOffset.UtcNow
                    });

                    var message = new MqttApplicationMessageBuilder()
                        .WithTopic(sc.Topic)
                        .WithPayload(payload)
                        .Build();

                    await mqttClient.PublishAsync(message);
                    Console.WriteLine($"Опубліковано в [{sc.Topic}]: {payload}");
                }

                await Task.Delay(TimeSpan.FromSeconds(2));
            }

            await mqttClient.DisconnectAsync();
            Console.WriteLine("Відключено від брокера.");
        }
    }
}
