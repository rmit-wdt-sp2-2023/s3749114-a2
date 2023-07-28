using System.Globalization;
using System.Net.Http.Headers;
using System.Net.Mime;

// Set culture info to ensure appropriate money symbol.

CultureInfo cultureInfo = new("en-AU");
cultureInfo.NumberFormat.CurrencySymbol = "$";
CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient("api", client =>
{
    client.BaseAddress = new Uri("http://localhost:5200");
    client.DefaultRequestHeaders.Accept.Add(
        new MediaTypeWithQualityHeaderValue(MediaTypeNames.Application.Json));
});

builder.Services.AddControllersWithViews();

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.Cookie.IsEssential = true;
});

WebApplication app = builder.Build();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.UseSession();
app.MapDefaultControllerRoute();
app.Run();