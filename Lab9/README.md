# Лабораторна робота №9 — асинхронність у SCADA-проєкті

**Варіант №9:** система зрошення сільськогосподарських угідь.
**Виконав:** студент Шкурат К. Ю.

ASP.NET Core MVC (.NET 8), Entity Framework Core + SQL Server LocalDB, MQTTnet 4.

Папка містить повне рішення SCADA-системи (стан після ЛР8) з асинхронними змінами до лабораторних робіт №6 і №8.
Ті самі зміни внесено й у папки `Lab6`, `Lab7` і `Lab8`.

## Зміни до лабораторної роботи №6 — фоновий MQTT-сервіс

Файл `IrrigationScada.Web/Services/MqttListenerService.cs` (клас `MqttListenerService : BackgroundService`):

| Вимога | Реалізація |
|---|---|
| `ExecuteAsync` — `async Task`, без `.Wait()` | `protected override async Task ExecuteAsync(...)`; `await client.ConnectAsync(options, stoppingToken)`, `await client.SubscribeAsync(subscribeOptions, stoppingToken)` |
| Обробник повідомлень | `client.ApplicationMessageReceivedAsync += e => { ...розбір JSON, _cache.Update(...)...; return Task.CompletedTask; }` |
| Повторне підключення з паузою | `await Task.Delay(5000, stoppingToken)` замість `Thread.Sleep(5000)`; цикл завершується за `stoppingToken` замість `while (true)` |
| У вебпроєкті немає `.Wait()`, `.Result`, `.GetAwaiter().GetResult()`, `Thread.Sleep` | Перевірено пошуком по всьому `IrrigationScada.Web` |
| `Index()` і дію поточних значень у `SchemeController` не змінювати | Не змінювались (дія поточних значень у проєкті називається `Values()`) |

## Зміни до лабораторної роботи №8 — `Controllers/SchemeController.cs`

| Було | Стало |
|---|---|
| — | `using Microsoft.EntityFrameworkCore;` |
| `public IActionResult Save()` … `_db.SaveChanges();` | `public async Task<IActionResult> Save()` … `await _db.SaveChangesAsync();` |
| `public IActionResult History()` … `.ToList();` | `public async Task<IActionResult> History()` … `await …ToListAsync();` |

## Запуск і перевірка

1. Запустити MQTT-брокер Mosquitto на `127.0.0.1:1883`.
2. Створити/оновити базу даних: `Update-Database` (Package Manager Console, проєкт `IrrigationScada.Web`).
3. Запустити `IrrigationScada.Simulator` і `IrrigationScada.Web` (Multiple startup projects, F5).
4. Перевірити:
   - мнемосхема відкривається, показання оновлюються автоматично;
   - кнопка «Зберегти показання» відкриває сторінку «Історія показань»;
   - після повторних натискань нові записи з'являються зверху.
