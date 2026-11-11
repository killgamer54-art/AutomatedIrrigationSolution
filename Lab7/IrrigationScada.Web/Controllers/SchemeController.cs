using IrrigationScada.Web.Data;
using IrrigationScada.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace IrrigationScada.Web.Controllers;

public class SchemeController : Controller
{
    private readonly LiveValuesCache _cache;
    private readonly SchemeConfigService _scheme;
    private readonly ApplicationDbContext _db; // нове поле

    public SchemeController(LiveValuesCache cache, SchemeConfigService scheme, ApplicationDbContext db)
    {
        _cache = cache;
        _scheme = scheme;
        _db = db; // новий параметр та ініціалізація
    }

    // Мнемосхема
    public IActionResult Index()
    {
        return View(_scheme.GetSensors());
    }

    // Поточні значення для оновлення мнемосхеми (викликається з scheme.js)
    [HttpGet]
    public IActionResult Values()
    {
        return Json(_cache.GetAll());
    }

    [HttpPost]
    public IActionResult Save()
    {
        foreach (var r in _cache.GetAll())
        {
            _db.SensorSamples.Add(new SensorSample
            {
                Topic = r.Topic,
                Value = r.Value,
                Unit = r.Unit,
                MeasuredAtUtc = r.MeasuredAtUtc
            });
        }
        _db.SaveChanges();
        return RedirectToAction("History");
    }

    public IActionResult History()
    {
        var samples = _db.SensorSamples
            .OrderByDescending(s => s.Id)
            .Take(50)
            .ToList();
        return View(samples);
    }
}
