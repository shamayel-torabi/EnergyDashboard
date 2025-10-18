

namespace MessageBus;

public interface IPublisher
{
    Task PublishAsync<T>(T message) where T :IMessage;
}
