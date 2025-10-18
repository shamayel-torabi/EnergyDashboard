using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using MeterFailService.Application.Interfaces;
using MeterFailService.Infrastructure.Persistence;
using MeterFailService.Infrastructure.Services;
using HealthCheck;

namespace MeterFailService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        string appDbConnectionStrings = configuration.GetConnectionString("DefaultConnection");
        string notificationUrl = configuration.GetValue<string>("NotificationUrl");
        string NotificationGrpcUrl = configuration.GetValue<string>("NotificationGrpcUrl");
        string meterServiceUrl = configuration.GetValue<string>("MeterServiceUrl");

        services.AddMessageService(NotificationGrpcUrl);
        services.AddHostedService(sp =>
        {
            return new ReceiveRefreshEnergyProfile(sp, notificationUrl);
        });

        services.AddHealthChecks()
            .AddCheck("سرویس  صحت میتر", () => HealthCheckResult.Healthy(), new string[] { "self" })
            .AddMemoryHealthCheck("بررسی حافظه", HealthStatus.Unhealthy, 4L * 1024L * 1024L * 1024L, new string[] { "self" })
            .AddDbContextCheck<ApplicationDbContext>("بررسی اتصال بانک اطلاعاتی", HealthStatus.Unhealthy, new string[] { "service" });

        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseNpgsql(appDbConnectionStrings, sqlOptions =>
            {
                sqlOptions.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName);
                sqlOptions.EnableRetryOnFailure(15, TimeSpan.FromSeconds(30), null);
            })
            .UseSnakeCaseNamingConvention();
        });

        services.AddSingleton<ICryptoService, CryptoService>();
        services.AddScoped<IApplicationDbContext>(provider => provider.GetService<ApplicationDbContext>());
        services.AddGrpcMeterService(meterServiceUrl);

        return services;
    }
}
