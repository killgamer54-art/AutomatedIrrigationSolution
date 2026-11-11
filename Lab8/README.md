# Лабораторна робота №8 — збереження історії показань датчиків у БД SCADA-системи

**Варіант №9:** система зрошення сільськогосподарських угідь.
ASP.NET Core MVC (.NET 8), Individual Accounts, Entity Framework Core + SQL Server LocalDB, MQTTnet.

## Склад рішення

| Проєкт | Призначення |
|---|---|
| `IrrigationScada.Components` | Бібліотека класів: `SensorReading`, MQTT-теми датчиків |
| `IrrigationScada.Simulator` | Симулятор об'єкта: публікує показання в MQTT кожні 2 с |
| `IrrigationScada.Web` | SCADA-вебзастосунок: мнемосхема, годинник, календар, історія |

## Зміни лабораторної роботи №8

| Файл | Зміна |
|---|---|
| `Data/SensorSample.cs` | Клас-сутність запису історії (Id, Topic, Value, Unit, MeasuredAtUtc) |
| `Data/ApplicationDbContext.cs` | `DbSet<SensorSample> SensorSamples` |
| `Data/Migrations/20261020120000_AddSensorSamples.cs` | Міграція: таблиця `dbo.SensorSamples` |
| `Controllers/SchemeController.cs` | Ін'єкція `ApplicationDbContext`, дії `Save` (POST) і `History` (GET) |
| `Views/Scheme/Index.cshtml` | Кнопка «Зберегти показання», атрибут `data-max` |
| `Views/Scheme/History.cshtml` | Таблиця останніх 50 записів |
| `SensorView.cs`, `Services/SchemeConfigService.cs` | Поріг `Max` для кожного датчика (температура — 28 °C) |
| `wwwroot/js/scheme.js`, `wwwroot/css/site.css` | Аварійне підсвічування датчика (клас `alarm`) |

## Асинхронні зміни (документ «Асинхронність у SCADA-проєкті»)

| Файл | Було | Стало |
|---|---|---|
| `Controllers/SchemeController.cs` | — | `using Microsoft.EntityFrameworkCore;` |
| `Controllers/SchemeController.cs` | `IActionResult Save()` … `_db.SaveChanges();` | `async Task<IActionResult> Save()` … `await _db.SaveChangesAsync();` |
| `Controllers/SchemeController.cs` | `IActionResult History()` … `.ToList();` | `async Task<IActionResult> History()` … `await …ToListAsync();` |
| `Services/MqttListenerService.cs` | пауза `Task.Delay(TimeSpan.FromSeconds(5))` | `await Task.Delay(5000, stoppingToken)`; у вебпроєкті немає `.Wait()`, `.Result`, `Thread.Sleep` |

## Запуск

1. Запустити MQTT-брокер Mosquitto на `127.0.0.1:1883`.
2. Створити базу даних (Package Manager Console, проєкт `IrrigationScada.Web`):
   ```
   Update-Database
   ```
   або `dotnet ef database update --project IrrigationScada.Web`.
3. Запустити `IrrigationScada.Simulator` і `IrrigationScada.Web` (Multiple startup projects).
4. Відкрити «Мнемосхема» → «Зберегти показання» → сторінка «Історія».
