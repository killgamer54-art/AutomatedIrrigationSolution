# Лабораторна робота №5 — бібліотека класів компонентів АСК

**Варіант №9:** система зрошення сільськогосподарських угідь.

Рішення `AutomatedIrrigationSolution.sln` складається з двох проєктів:

| Проєкт | Тип | Призначення |
|---|---|---|
| `IrrigationDomainLibrary` | Class Library | Сенсори, виконавчі механізми, контролер |
| `IrrigationConsoleApp` | Console App | Цикл сканування та публікація телеметрії через MQTT (MQTTnet) |

Бібліотека підключена до застосунку через `<ProjectReference>`.

```
AutomatedIrrigationSolution.sln
├── IrrigationDomainLibrary/
│   ├── Sensors/       SoilMoistureSensor.cs, PressureSensor.cs
│   ├── Actuators/     IrrigationValve.cs, WaterPump.cs
│   └── Controllers/   IrrigationController.cs
└── IrrigationConsoleApp/
    └── Program.cs     MQTT-клієнт і цикл публікації
```

## Запуск

```bash
dotnet run --project IrrigationConsoleApp            # брокер broker.hivemq.com:1883
dotnet run --project IrrigationConsoleApp -- localhost 1883
```

Телеметрія публікується в топік `ikm/irrigation/variant9/telemetry` у форматі JSON.
