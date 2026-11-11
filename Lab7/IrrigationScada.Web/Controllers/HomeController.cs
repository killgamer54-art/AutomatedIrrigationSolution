using Microsoft.AspNetCore.Mvc;

namespace IrrigationScada.Web.Controllers;

public class HomeController : Controller
{
    public IActionResult Index() => View();

    public IActionResult Error() => Content("Сталася помилка під час обробки запиту.");
}
