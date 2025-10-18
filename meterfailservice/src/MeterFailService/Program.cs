
using EnergyDashboard.Common.Data;
using HealthCheck;
using MeterFailService.Common.Logging;
using MeterFailService.Application;
using MeterFailService.Infrastructure;
using MeterFailService.Infrastructure.Persistence;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Serilog;
using System.Globalization;
using MeterFailService.Endpoints;

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var seed = args.Contains("/seed");
if (seed)
{
    args = args.Except(new[] { "/seed" }).ToArray();
}

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog(SerilogConfig.LogConfigure);

builder.Services.AddApplication(builder.Configuration);
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddHttpContextAccessor();

builder.Services.AddCors(o => o.AddPolicy("ApiCorsPolicy", builder =>
{
    builder.AllowAnyOrigin()
           .AllowAnyMethod()
           .AllowAnyHeader();
}));

builder.Services.AddMemoryCache();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    app.UseDeveloperExceptionPage();
}

app.UseCors("ApiCorsPolicy");
app.UseDefaultFiles();
app.UseStaticFiles();
app.UseRouting();

app.MapHealthChecks("/health", new HealthCheckOptions { Predicate = r => r.Tags.Contains("service") });
app.MapHealthChecks("/liveness", new HealthCheckOptions  { Predicate = r => r.Tags.Contains("self") });
app.MapHealthChecks("/healthz", new HealthCheckOptions()
{
    Predicate = r => true,
    ResponseWriter = UIResponseWriter.WriteResponse
});

app.MapMetersEnergyEndpoints();

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

    app.Logger.LogInformation($"Starting MeterFailService at {DateTime.Now} ...");

    app.Run();
    return 0;
}
catch (Exception ex)
{
    Log.Error(ex, "MeterFailService terminated unexpectedly.");
    Console.WriteLine(ex.Message);
    return 1;
}
finally
{
    Log.CloseAndFlush();
}

