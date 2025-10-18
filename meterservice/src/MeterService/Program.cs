using Serilog;
using System.Globalization;
using HealthCheck;
using MeterService.API.Services;
using MeterService.Application;
using MeterService.Application.Interfaces;
using MeterService.Infrastructure;
using MeterService.Infrastructure.Persistence;
using MeterService.Services;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using EnergyDashboard.Common.Data;
using EnergyDashboard.Common.Logging;

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var seed = args.Contains("/seed");
if (seed)
{
    args = args.Except(new[] { "/seed" }).ToArray();
}

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog(SerilogConfig.LogConfigure);
builder.Services.AddGrpc();

builder.Services.AddApplication(builder.Configuration);
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<ICurrentUserService, CurrentUserService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}


app.UseRouting();


app.MapGrpcService<GrpcMeterService>();
app.MapHealthChecks("/health", new HealthCheckOptions { Predicate = r => r.Tags.Contains("service") });
app.MapHealthChecks("/liveness", new HealthCheckOptions  { Predicate = r => r.Tags.Contains("self") });
app.MapHealthChecks("/healthz", new HealthCheckOptions()
{
    Predicate = r => true,
    ResponseWriter = UIResponseWriter.WriteResponse
});

app.MapGet("/", () => "Healthy");

var cultureInfo = new CultureInfo("fa-IR");
CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

try
{
    if (seed)
    {
        app.MigrateDbContext<ApplicationDbContext>((context, services) =>
        {
        });
        return 0;
    }

    app.Logger.LogInformation($"Starting MeterService Grpc at {DateTime.Now} ...");

    app.Run();
    return 0;
}
catch (Exception ex)
{
    Log.Error(ex, "MeterService terminated unexpectedly.");
    Console.WriteLine(ex.Message);
    return 1;
}
finally
{
    Log.CloseAndFlush();
}

