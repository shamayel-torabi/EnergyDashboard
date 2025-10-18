
namespace MessageBus;

public interface IMessageBusProvider : IPublisher
{
    Task StartSubscription(CancellationToken stoppingToken);
}
