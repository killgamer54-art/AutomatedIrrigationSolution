# Лабораторна робота №4 — обмін даними за протоколом MQTT

**Варіант №9:** система зрошення сільськогосподарських угідь (C#, .NET 8, MQTTnet 5.2.0.1603).

Застосунок підключається до локального брокера Mosquitto (`127.0.0.1:1883`) і 10 разів
з інтервалом 2 с публікує показання чотирьох сенсорів у форматі JSON.

## MQTT-теми

| Сенсор | Тема | Одиниця |
|---|---|---|
| Вологість ґрунту | `irrigation/9/sensors/soil_moisture` | % |
| Тиск у магістралі | `irrigation/9/sensors/pressure` | bar |
| Витрата води | `irrigation/9/sensors/flow_rate` | m3/h |
| Температура повітря | `irrigation/9/sensors/temperature` | C |

Формат повідомлення:

```json
{"value":45.5,"unit":"%","measuredAtUtc":"2026-10-22T14:00:00+00:00"}
```

## Запуск

1. Запустити брокер Mosquitto на `127.0.0.1:1883`.
2. Підписатися на теми (PowerShell):
   ```powershell
   & "C:\Program Files\mosquitto\mosquitto_sub.exe" -h 127.0.0.1 -p 1883 -t "irrigation/9/sensors/#" -v
   ```
3. Запустити застосунок:
   ```bash
   dotnet run --project IrrigationMqttTelemetry
   ```
