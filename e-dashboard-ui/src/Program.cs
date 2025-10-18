
using Serilog;
using System.Globalization;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using HealthCheck;
using EnergyDashboard.WebClient.Logging;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog(SerilogConfig.LogConfigure);

string redisConnection = builder.Configuration.GetConnectionString("RedisConnection");
string connectionStrings = builder.Configuration.GetConnectionString("DefaultConnection");

string energyDashboardUrl = builder.Configuration.GetValue<string>("EnergyDashboardUrl");
string notificationUrl = builder.Configuration.GetValue<string>("NotificationUrl");
string meterServiceUrl = builder.Configuration.GetValue<string>("MeterServiceUrl");
string identityServerUrl = builder.Configuration.GetValue<string>("IdentityServerUrl");

builder.Services.AddHealthChecks()
    .AddCheck("داشبورد انرژی", () => HealthCheckResult.Healthy("داشبورد انرژی آماده خدمت رسانی است"), new string[] {"self"})
    .AddMemoryHealthCheck(name:"بررسی حافظه", HealthStatus.Unhealthy, 1L * 1024L * 1024L * 1024L, new string[] { "self" })
    .AddCockroachDb("http://cockroachdb-db:8080", "بررسی اتصال به بانک اطلاعاتی", HealthStatus.Unhealthy, new string[] { "service" })
    .AddRedis(redisConnection, "بررسی اتصال به سرور ردیس", HealthStatus.Unhealthy, new string[] { "service" })
    .AddHttpHealthCheck(op =>
    {
        op.AddHost("سرویس احراز هویت", new Uri($"{identityServerUrl}/health"));
        op.AddHost("سرویس پیام رسان", new Uri($"{notificationUrl}/health"));
        op.AddHost("سرویس میتر", new Uri($"{meterServiceUrl}/health"));
        op.AddHost("سرویس داشبورد انرژی", new Uri($"{energyDashboardUrl}/health"));
        op.AddHost("سرویس رخداد نگاری", new Uri("http://seq-logger"));
        op.AddHost("سپاک", new Uri("https://mwsmeteringamr-wr.igmc.ir/MeteringDataService.svc"));
        op.AddHost("مدام", new Uri("https://mwsmeteringmodam.igmc.ir"));
    }, "بررسی اتصال شبکه", HealthStatus.Unhealthy, new string[] {"network"} );


builder.Services.AddCors(o => o.AddPolicy("ApiCorsPolicy", builder =>
{
    builder.AllowAnyOrigin()
           .AllowAnyMethod()
           .AllowAnyHeader();
}));

var app = builder.Build();

app.UseStaticFiles();
app.UseCors("ApiCorsPolicy");
app.UseRouting();

app.MapHealthChecks("/health", new HealthCheckOptions { Predicate = r => r.Tags.Contains("service") });
app.MapHealthChecks("/liveness", new HealthCheckOptions { Predicate = r => r.Tags.Contains("self") });
app.MapHealthChecks("/healthz", new HealthCheckOptions()
{
    Predicate = r => true,
    ResponseWriter = UIResponseWriter.WriteResponse
}) ;

app.MapFallbackToFile("index.html");

var cultureInfo = new CultureInfo("fa-IR");
CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

try
{
    app.Logger.LogInformation($"Starting EnergyDashboard WebClient at {DateTime.Now} ...");
    app.Run();
    return 0;
}
catch (Exception ex)
{
    app.Logger.LogCritical(ex, "Host terminated unexpectedly");
    Console.WriteLine(ex.Message);
    return 1;
}
finally
{
    Log.CloseAndFlush();
}
