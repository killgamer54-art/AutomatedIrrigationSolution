using IrrigationScada.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace IrrigationScada.Web.Controllers;

public class SchemeController : Controller
{
    private readonly LiveValuesCache _cache;
    private readonly SchemeConfigService _scheme;

    public SchemeController(LiveValuesCache cache, SchemeConfigService scheme)
    {
        _cache = cache;
        _scheme = scheme;
    }

    // Мнемосхема
    public IActionResult Index()
    {
        return View(_scheme.GetSensors());
    }

    // Поточні значення для оновлення мнемосхеми у форматі JSON (викликається з scheme.js)
    [HttpGet]
    public IActionResult Values()
    {
        return Json(_cache.GetAll());
    }
}
