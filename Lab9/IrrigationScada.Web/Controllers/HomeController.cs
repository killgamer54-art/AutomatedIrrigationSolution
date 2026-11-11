using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IrrigationScada.Web.Controllers;

public class HomeController : Controller
{
    // Без атрибутів — діє FallbackPolicy: потрібен вхід у систему
    public IActionResult Index() => View();

    // ЛР №9: сторінка доступна всім відвідувачам
    [AllowAnonymous]
    public IActionResult Privacy()
    {
        return View();
    }

    [AllowAnonymous]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error() => Content("Сталася помилка під час обробки запиту.");
}
