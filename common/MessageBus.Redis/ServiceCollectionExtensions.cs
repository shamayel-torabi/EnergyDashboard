
using MessageBus;
using MessageBus.Internal;
using MessageBus.Redis;
using StackExchange.Redis;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRedisMessageBus(this IServiceCollection services, Action<MessageBusConfiguration> setupAction)
    {
        var serviceConfig = new MessageBusConfiguration();
        setupAction.Invoke(serviceConfig);
        services.Configure(setupAction);

        if (serviceConfig.ConnectionString is null)
            throw new ArgumentNullException(nameof(serviceConfig.ConnectionString));

        services.AddSingleton<IConnectionMultiplexer>(x => ConnectionMultiplexer.Connect(serviceConfig.ConnectionString));
        services.AddSingleton<IMessageBusProvider, RedisMessageBusProvider>();
        services.AddSingleton<IPublisher, MessagePublisher>();

        if (serviceConfig.AssembliesToRegister.Any())
        {
            ServiceRegistrar.AddMediatRClasses(services, serviceConfig);
            ServiceRegistrar.AddRequiredServices(services, serviceConfig);

            services.AddHostedService<SubscriberHostedService>(sp =>
            {
                var messageSubscriber = sp.GetRequiredService<IMessageBusProvider>();
                return new SubscriberHostedService(messageSubscriber);
            });
        }

        return services;
    }
}