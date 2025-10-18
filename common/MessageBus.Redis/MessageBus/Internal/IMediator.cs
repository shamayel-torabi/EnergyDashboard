namespace MessageBus.Internal;

internal interface IMediator
{
    Task Publish<TMessage>(TMessage message, CancellationToken cancellationToken = default) where TMessage : IMessage;
}
