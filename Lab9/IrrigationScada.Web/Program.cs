using IrrigationScada.Web.Data;
using IrrigationScada.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// База даних SQL Server LocalDB + ASP.NET Core Identity (Individual Accounts)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddEntityFrameworkStores<ApplicationDbContext>();

// SCADA-сервіси: кеш оперативних значень, конфігурація мнемосхеми, MQTT-підписник
builder.Services.AddSingleton<LiveValuesCache>();
builder.Services.AddSingleton<SchemeConfigService>();
builder.Services.AddHostedService<MqttListenerService>();

builder.Services.AddControllersWithViews();

// ЛР №9: авторизація. «Закрито за замовчуванням» — кінцеві точки без власних правил
// доступу вимагають автентифікованого користувача; іменована політика для збереження показань.
builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();

    options.AddPolicy("CanReadSavings", policy =>
    {
        policy.RequireAuthenticatedUser();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication(); // хто звернувся (cookie Identity)
app.UseAuthorization();  // чи має він доступ до дії

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
