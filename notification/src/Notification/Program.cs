using System.Globalization;
using Microsoft.EntityFrameworkCore;
using Notification.Data;
using Notification.Hub;
using Serilog;
using EnergyDashboard.Common.Data;
using Notification.Logging;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using HealthCheck;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Notification.Services;

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);


var seed = args.Contains("/seed");
if (seed)
{
    args = args.Except(new[] { "/seed" }).ToArray();
}

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog(SerilogConfig.LogConfigure);

builder.Services.AddGrpc();

builder.Services.AddHealthChecks()
    .AddCheck("سرویس پیام رسان", () => HealthCheckResult.Healthy("سرویس پیام رسان آماده خدمت رسانی است"), new string[] { "self" })
    .AddMemoryHealthCheck("بررسی حافظه",  HealthStatus.Unhealthy,  1L * 1024L * 1024L * 1024L,  new string[] { "self" })
    .AddDbContextCheck<MessageDbContext>("بررسی اتصال به بانک اطلاعاتی", HealthStatus.Unhealthy, new string[] { "service" });

builder.Services.AddDbContext<MessageDbContext>(options =>
{
    string? connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

    options.UseNpgsql(connectionString, sqlOptions =>
    {
        sqlOptions.MigrationsAssembly(typeof(MessageDbContext).Assembly.FullName);
        sqlOptions.EnableRetryOnFailure(15, TimeSpan.FromSeconds(30), null);
    })
    .UseSnakeCaseNamingConvention();
});

builder.Services.AddSignalR();

builder.Services.AddCors(o => o.AddPolicy("ApiCorsPolicy", builder =>
{
    builder.AllowAnyMethod()
           .AllowAnyHeader()
           .SetIsOriginAllowed(_ => true)
           .AllowCredentials();
}));


var app = builder.Build();

app.UseCors("ApiCorsPolicy");
app.UseStaticFiles();
app.UseRouting();

app.MapHealthChecks("/health");
app.MapHealthChecks("/liveness", new HealthCheckOptions { Predicate = r => r.Tags.Contains("self") });
app.MapHealthChecks("/healthz", new HealthCheckOptions()
{
    ResponseWriter = UIResponseWriter.WriteResponse
}); 

app.MapGrpcService<NotificationService>();
app.MapHub<MessageHub>("/message");
app.MapGet("/", () => "Healthy");

var cultureInfo = new CultureInfo("fa-IR");
CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

try
{
    if (seed)
    {
        app.MigrateDbContext<MessageDbContext>((context, services) =>
        {
        });

        return 0;
    }

    app.Logger.LogInformation($"Starting Notification at {DateTime.Now} ...");
    app.Run();
    return 0;
}
catch (Exception ex)
{
    app.Logger.LogCritical(ex, "Notification Host terminated unexpectedly");
    Console.WriteLine(ex.Message);
    return 1;
}
finally
{
    Log.CloseAndFlush();
}

