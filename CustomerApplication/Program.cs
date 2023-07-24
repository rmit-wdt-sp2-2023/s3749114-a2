using System.Globalization;
using CustomerApplication.Data;
using CustomerApplication.Services;
using CustomerApplication.BackgroundServices;
using Microsoft.EntityFrameworkCore;

CultureInfo cultureInfo = new("en-AU");
cultureInfo.NumberFormat.CurrencySymbol = "$";
CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add services to container.

builder.Services.AddDbContext<BankContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("BankContext"));
    options.UseLazyLoadingProxies();
});

builder.Services.AddHostedService<BillPayBackgroundService>();

// Store session and make cookie essential.

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.Cookie.IsEssential = true;
});

builder.Services.AddScoped<BankService>();

builder.Services.AddControllersWithViews();

WebApplication app = builder.Build();

// Seed data.

using (var scope = app.Services.CreateScope())
{
    IServiceProvider services = scope.ServiceProvider;
    try
    {
        SeedData.Initialise(services);
    }
    catch (Exception ex)
    {
        ILogger logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred seeding the database.");
    }
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.UseSession();
app.MapDefaultControllerRoute();
app.Run();