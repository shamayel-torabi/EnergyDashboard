

namespace MessageBus;

public class MessagePublisher : IPublisher
{
    private readonly IMessageBusProvider _messageBusProvider;

    public MessagePublisher(IMessageBusProvider messageBusProvider)
    {
        _messageBusProvider = messageBusProvider;
    }

    public async Task PublishAsync<T>(T message) where T : IMessage
    {
       await _messageBusProvider.PublishAsync(message);
    }
}
