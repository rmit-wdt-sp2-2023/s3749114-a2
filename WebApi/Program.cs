using Microsoft.EntityFrameworkCore;
using BankLibrary.Data;
using WebApi.Models.DataManager;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<BankContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("BankContext"),
       assembly => assembly.MigrationsAssembly(typeof(BankContext).Assembly.FullName));
});

builder.Services.AddScoped<CustomerManager>();
builder.Services.AddScoped<LoginManager>();
builder.Services.AddScoped<BillPayManager>();

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

WebApplication app = builder.Build();

app.MapControllers();

app.Run();