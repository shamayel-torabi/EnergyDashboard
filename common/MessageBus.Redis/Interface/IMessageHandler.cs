namespace MessageBus;

public interface IMessageHandler<in TMessage> where TMessage : IMessage
{
    Task Handle(TMessage message, CancellationToken cancellationToken);
}
