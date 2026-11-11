# Лабораторна робота №6 — мнемосхема технологічного процесу у вебінтерфейсі SCADA-системи

**Варіант №9:** система зрошення сільськогосподарських угідь.
**Виконав:** студент Шкурат К. Ю.

ASP.NET Core MVC (.NET 8, шаблон Individual Accounts), MQTTnet 4, MQTT-брокер Mosquitto.

## Склад рішення `IrrigationScada.sln`

| Проєкт | Призначення |
|---|---|
| `IrrigationScada.Components` | Бібліотека класів: модель показання `SensorReading`, формат повідомлення `SensorPayload`, MQTT-топіки `SensorTopics` |
| `IrrigationScada.Simulator` | Симулятор об'єкта: кожні 2 с публікує показання 4 датчиків у MQTT |
| `IrrigationScada.Web` | SCADA-вебзастосунок (MVC) з мнемосхемою; посилається на `IrrigationScada.Components` |

```
IrrigationScada.Web/
├── Controllers/SchemeController.cs     Index — сторінка мнемосхеми; Values — поточні показання у JSON
├── Services/MqttListenerService.cs     фоновий сервіс: підписка на irrigation/9/sensors/#
├── Services/LiveValuesCache.cs         поточні значення в пам'яті (ConcurrentDictionary)
├── Services/SchemeConfigService.cs     розташування індикаторів на мнемосхемі
├── SensorView.cs                       опис індикатора (назва, топік, одиниця, X, Y)
├── Views/Scheme/Index.cshtml           мнемосхема: насос → магістраль → клапан → поле
├── wwwroot/js/scheme.js                оновлення показань кожні 2 с без перезавантаження
└── appsettings.json                    Mqtt:Host = 127.0.0.1, Mqtt:Port = 1883
```

## MQTT-топіки

| Датчик | Топік | Одиниця |
|---|---|---|
| Вологість ґрунту | `irrigation/9/sensors/soil_moisture` | % |
| Тиск у магістралі | `irrigation/9/sensors/pressure` | bar |
| Витрата води | `irrigation/9/sensors/flow_rate` | m3/h |
| Температура повітря | `irrigation/9/sensors/temperature` | C |

Формат повідомлення: `{"value":45.5,"unit":"%","measuredAtUtc":"2026-10-22T14:00:00+00:00"}`

## Запуск

1. Запустити MQTT-брокер Mosquitto на `127.0.0.1:1883`.
2. Створити базу Identity: `Update-Database` (Package Manager Console, проєкт `IrrigationScada.Web`).
3. Запустити `IrrigationScada.Simulator` і `IrrigationScada.Web` (Multiple startup projects, F5).
4. Відкрити «Мнемосхема» — значення датчиків оновлюються кожні 2 с.
