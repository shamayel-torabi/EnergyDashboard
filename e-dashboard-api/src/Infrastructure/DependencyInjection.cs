using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using EnergyDashboard.Infrastructure.Persistence;
using EnergyDashboard.Infrastructure.Services;
using EnergyDashboard.Application.Interfaces;
using HealthCheck;
using EnergyDashboard.Infrastructure.Persistence.Interceptors;
using EnergyDashboard.Domain.Repository;
using EnergyDashboard.Infrastructure.Repository;

namespace EnergyDashboard.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        string appDbConnectionStrings = configuration.GetConnectionString("AppDbConnection");
        string redisConnection = configuration.GetConnectionString("RedisConnection");
        string meterServiceUrl = configuration.GetValue<string>("MeterServiceUrl");
        string notificationUrl = configuration.GetValue<string>("NotificationUrl");
        string notificationGrpcUrl = configuration.GetValue<string>("NotificationGrpcUrl");


        services.AddHealthChecks()
            .AddCheck("سرویس داشبورد انرژی", () => HealthCheckResult.Healthy("سرویس داشبورد انرژی آماده خدمات رسانی است"), new string[] { "self" })
            .AddMemoryHealthCheck("بررسی حافظه", HealthStatus.Unhealthy, 1L * 1024L * 1024L * 1024L, new string[] { "self" })
            .AddDbContextCheck<AppDbContext>("بررسی اتصال به بانک اطلاعاتی", HealthStatus.Unhealthy, new string[] { "service" })
            .AddRedis(redisConnection, "بررسی اتصال به سرور ردیس", HealthStatus.Unhealthy, new string[] { "service" });

        services.AddHostedService(sp =>
        {
            return new ReceiveRefreshEnergyProfile(sp, notificationUrl);
        });


        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = redisConnection;
        });

        services.AddScoped<AuditableEntitySaveChangesInterceptor>();

        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseNpgsql(appDbConnectionStrings, sqlOptions =>
            {
                sqlOptions.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName);
                sqlOptions.EnableRetryOnFailure(15, TimeSpan.FromSeconds(30), null);
            })
            .UseSnakeCaseNamingConvention();
        });


        services.AddScoped<IUnitOfWork>(provider => provider.GetService<AppDbContext>());

        services.AddScoped<IAreaRepository, AreaRepository>();
        services.AddScoped<IDailyEnergyRepository, DailyEnergyRepository>();
        services.AddScoped<IEnergyProfileRepository, EnergyProfileRepository>();
        services.AddScoped<IEnergyTariffRepository, EnergyTariffRepository>();
        services.AddScoped<IEquipmentRepository, EquipmentRepository>();
        services.AddScoped<ILoadFeederTypeRepository, LoadFeederTypeRepository>();
        services.AddScoped<IMeterReadingFailureRepository, MeterReadingFailureRepository>();
        services.AddScoped<INetworkRepository, NetworkRepository>();
        services.AddScoped<IPowerplantOperatorRepository, PowerplantOperatorRepository>();
        services.AddScoped<IPowerplantTypeRepository, PowerplantTypeRepository>();
        services.AddScoped<ISubstationRepository, SubstationRepository>();
        services.AddScoped<IZoneRepository, ZoneRepository>();

        services.AddSingleton<ICryptoService, CryptoService>();
        services.AddScoped<IEquipmentEnergyService, EquipmentEnergyService>();
        services.AddScoped<IMeterEnergyService, MeterEnergyService>();
        services.AddScoped<IMeterInstantService, MeterInstantService>();
        services.AddScoped<IDateTimeService, DateTimeService>();

        services.AddMessageService(notificationGrpcUrl);
        services.AddGrpcMeterService(meterServiceUrl);
        services.AddJobScheduledService(Assembly.GetExecutingAssembly());

        return services;
    }
}