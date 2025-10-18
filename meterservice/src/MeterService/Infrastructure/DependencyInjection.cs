using System.Text;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using MeterService.Application.Interfaces;
using MeterService.Application.Models;
using MeterService.Infrastructure.Persistence;
using MeterService.Infrastructure.Services;
using Microsoft.Extensions.Options;
using HealthCheck;
using Infrastructure.Common;
using Notification.gRPC.Services;
using Microsoft.Extensions.Caching.Distributed;

namespace MeterService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        string appDbConnectionStrings = configuration.GetConnectionString("DefaultConnection");
        string redisConnection = configuration.GetConnectionString("RedisConnection");

        string notificationUrl = configuration.GetValue<string>("NotificationUrl");
        string identityServerUrl = configuration.GetValue<string>("IdentityServerUrl");

        services.Configure<IGMCOptions>(configuration.GetSection("IGMCUser"));

        services.AddMessageService(notificationUrl);
        services.AddDateTimeService();

        services.AddHealthChecks()
            .AddCheck("سرویس میتر", () => HealthCheckResult.Healthy(), new string[] {"self"})
            .AddMemoryHealthCheck("بررسی حافظه", HealthStatus.Unhealthy, 4L * 1024L * 1024L * 1024L, new string[] { "self" })
            .AddDbContextCheck<ApplicationDbContext>("بررسی اتصال بانک اطلاعاتی", HealthStatus.Unhealthy, new string[] { "service" })
            .AddRedis(redisConnection, "بررسی اتصال به سرور ردیس", HealthStatus.Unhealthy, new string[] { "service" })
            .AddHttpHealthCheck(op =>
             {
                 op.AddHost("سرویس احراز هویت", new Uri($"{identityServerUrl}/health"));
                 op.AddHost("سرویس پیام رسان", new Uri($"{notificationUrl}/health"));
                 op.AddHost("سرویس رخداد نگاری", new Uri("http://seq-logger"));
                 op.AddHost("سپاک", new Uri("https://mwsmeteringamr-wr.igmc.ir/MeteringDataService.svc"));
                 op.AddHost("مدام", new Uri("https://mwsmeteringmodam.igmc.ir"));
             }, "بررسی اتصال شبکه", HealthStatus.Unhealthy, new string[] { "network" });

        // services.AddRedisMessageBus(option =>
        // {
        //     option.ConnectionString = redisConnection;
        // });

        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = redisConnection;
        });

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
        services.AddScoped<IDomainEventService, DomainEventService>();

        services.AddModamService();
        services.AddSepacService();

        services.AddJobScheduledService(Assembly.GetExecutingAssembly());
        services.AddBackgroundTaskQueue();

        return services;
    }

    private static IServiceCollection AddSepacService(this IServiceCollection services)
    {
        services.AddSingleton<IMeteringDataService, MeteringDataServiceClient>(sp =>
        {
            IOptions<IGMCOptions> option = sp.GetRequiredService<IOptions<IGMCOptions>>();

            var client = new MeteringDataServiceClient();

            client.ClientCredentials.UserName.UserName = Environment.GetEnvironmentVariable("IGMC_USERNAME");
            client.ClientCredentials.UserName.Password = Environment.GetEnvironmentVariable("IGMC_PASSWORD");

            client.Endpoint.Binding.ReceiveTimeout = TimeSpan.FromMinutes(option.Value.Timeout);
            client.Endpoint.Binding.SendTimeout = TimeSpan.FromMinutes(option.Value.Timeout);
            return client;
        });

        services.AddSingleton<ISepacService, SepacService>();

        return services;
    }

    private static IServiceCollection AddModamService(this IServiceCollection services)
    {
        services.AddHttpClient<IModamService, ModamService>();

        services.AddSingleton<IModamService, ModamService>(sp => {
            var logger = sp.GetService<ILogger<ModamService>>();
            var option = sp.GetService<IOptions<IGMCOptions>>();
            var clientFactory = sp.GetService<IHttpClientFactory>();
            var messageService = sp.GetService<IMessageService>();
            var cache = sp.GetService<IDistributedCache>();

            string username = Environment.GetEnvironmentVariable("IGMC_USERNAME");
            string password = Environment.GetEnvironmentVariable("IGMC_PASSWORD");
            string url = option.Value.ModamUrl.TrimEnd('/');

            var byteArray = Encoding.ASCII.GetBytes($"{username}:{password}");
            var httpClient = clientFactory.CreateClient();
            httpClient.BaseAddress = new Uri(url);
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

            return new ModamService(logger, httpClient, messageService, cache);
        });
        return services;
    }

    private static IServiceCollection AddBackgroundTaskQueue(this IServiceCollection services)
    {
        services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>(sp =>
        {
            var backgroundTaskQueue = new BackgroundTaskQueue(256);
            return backgroundTaskQueue;
        });

        services.AddHostedService<QueuedHostedService>();

        return services;
    }
}
