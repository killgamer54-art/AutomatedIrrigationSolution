using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace IrrigationScada.Web.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // Лабораторна робота №8: таблиця історії показань датчиків
    public DbSet<SensorSample> SensorSamples => Set<SensorSample>();
}
