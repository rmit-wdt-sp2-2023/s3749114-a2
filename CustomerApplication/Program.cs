using System.Globalization;
using BankLibrary.Data;
using CustomerApplication.Data;
using CustomerApplication.Services;
using CustomerApplication.BackgroundServices;
using Microsoft.EntityFrameworkCore;

// Set culture info to ensure appropriate money symbol.

CultureInfo cultureInfo = new("en-AU");
cultureInfo.NumberFormat.CurrencySymbol = "$";
CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add database.

builder.Services.AddDbContext<BankContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("BankContext"));
    options.UseLazyLoadingProxies();
});

// Add BillPay background service.

builder.Services.AddHostedService<BillPayBackgroundService>();

// Store session and make cookie essential.

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.Cookie.IsEssential = true;
});

// Add other banking services. 

builder.Services.AddScoped<TransactionService>();
builder.Services.AddScoped<BillPayService>();
builder.Services.AddScoped<AccountService>();
builder.Services.AddScoped<CustomerService>();
builder.Services.AddScoped<LoginService>();

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