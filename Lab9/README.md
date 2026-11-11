# Лабораторна робота №9 — автентифікація та авторизація користувачів і захист сторінок SCADA-системи

**Варіант №9:** система зрошення сільськогосподарських угідь.
**Виконав:** студент Шкурат К. Ю.

ASP.NET Core MVC (.NET 8), ASP.NET Core Identity (Individual Accounts), EF Core + SQL Server LocalDB, MQTTnet 4.
Рішення продовжує лабораторну роботу №8 (папка `Lab8`, з асинхронними змінами). Симулятор, бібліотека класів і структура БД не змінювались.

## Змінені файли вебпроєкту `IrrigationScada.Web`

| Файл | Зміна |
|---|---|
| `Program.cs` | `AddAuthorization`: `FallbackPolicy` (`RequireAuthenticatedUser`) і іменована політика `CanReadSavings`; явний порядок `UseRouting → UseAuthentication → UseAuthorization` |
| `Controllers/SchemeController.cs` | `[Authorize]` на класі; над `Save()` — `[HttpPost]`, `[ValidateAntiForgeryToken]`, `[Authorize(Policy = "CanReadSavings")]` |
| `Controllers/HomeController.cs` | `[AllowAnonymous]` над `Privacy()` і `Error()` (+ `[ResponseCache]` над `Error()`); `Index()` закрита через `FallbackPolicy` |
| `Views/Shared/_Layout.cshtml` | Посилання «Мнемосхема» та «Історія» показуються лише автентифікованим користувачам; посилання «Конфіденційність» — усім |
| `wwwroot/js/scheme.js` | У `refresh()` після запиту поточних значень: якщо `response.redirected` — `window.location.reload()` (сеанс завершився) |

Додатково створено `Views/Home/Privacy.cshtml` — у проєкті не було сторінки Privacy, яку за завданням потрібно відкривати без входу.

Дія поточних значень датчиків у цьому проєкті називається `Values` (`/Scheme/Values`) — це аналог `Current` з методички.

## Перевірка

1. Запустити Mosquitto (`127.0.0.1:1883`), `IrrigationScada.Simulator` і `IrrigationScada.Web` (F5). За потреби — `Update-Database`.
2. У приватному вікні: `/Scheme`, `/Scheme/History`, `/Scheme/Values` → сторінка Login з `ReturnUrl`; після входу — повернення на запитану сторінку.
3. Без входу `/Home/Privacy` відкривається, `/Home/Index` перенаправляє на Login.
4. Після входу в меню з'являються «Мнемосхема» та «Історія»; «Зберегти показання» записує дані.
5. Видалити у DevTools поле `__RequestVerificationToken` з форми й натиснути «Зберегти показання» → HTTP 400.
6. Видалити cookie `.AspNetCore.Identity.Application` на відкритій мнемосхемі → після наступного опитування сторінка перезавантажується і відкривається Login.
