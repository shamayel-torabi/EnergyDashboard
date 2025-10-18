using MessageBus.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using System.Text.Json;

namespace MessageBus.Redis;

public class RedisMessageBusProvider : IMessageBusProvider
{
    private readonly ISubscriber _subscriber;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<RedisMessageBusProvider> _logger;
    private readonly MessageBusConfiguration _messageBusConfiguration;

    public RedisMessageBusProvider(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        var connectionMultiplexer = serviceProvider.GetRequiredService<IConnectionMultiplexer>();
        _subscriber = connectionMultiplexer.GetSubscriber();
        var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
        _logger = loggerFactory.CreateLogger<RedisMessageBusProvider>();
        var config = _serviceProvider.GetRequiredService<IOptions<MessageBusConfiguration>>();
        _messageBusConfiguration = config.Value;
    }

    public async Task PublishAsync<T>(T message) where T : IMessage
    {
        try
        {
            var eventName = typeof(T).Name;
            JsonSerializerOptions options = new JsonSerializerOptions { IncludeFields = true };
            string jsonString = JsonSerializer.Serialize<T>(message, options);
            RedisValue payload = new RedisValue(jsonString);

            await _subscriber.PublishAsync(eventName, payload);
        }
        catch (Exception ex)
        {
            _logger.LogError("Redis Publish Error", ex);
        }
    }

    public async Task StartSubscription(CancellationToken stoppingToken)
    {
        try
        {
            //var serviceProvider = _serviceProvider.CreateScope().ServiceProvider;
            //var mediator = serviceProvider.GetRequiredService<IMediator>();

            var mediator = _serviceProvider.GetRequiredService<IMediator>();

            var assembliesToScan = _messageBusConfiguration.AssembliesToRegister.Distinct().ToArray();

            var type = typeof(IMessage);
            var messageTypes = assembliesToScan
                .SelectMany(s => s.GetTypes())
                .Where(p => type.IsAssignableFrom(p));

            foreach (Type messageType in messageTypes)
            {
                Action<RedisChannel, RedisValue> action = async (channel, value) =>
                {
                    string eventName = channel.ToString();
                    JsonSerializerOptions options = new JsonSerializerOptions { IncludeFields = true };
                    IMessage? message = JsonSerializer.Deserialize(value.ToString(), messageType, options) as IMessage;

                    if (message is not null)
                    {
                        await mediator.Publish(message, stoppingToken);
                    }
                };

                await _subscriber.SubscribeAsync(messageType.Name, action);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("StartSubscription Error", ex);
        }
    }
}
