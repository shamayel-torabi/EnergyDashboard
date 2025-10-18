namespace MessageBus.Internal;

internal interface IMessagePublisher
{
    Task Publish(IEnumerable<MessageHandlerExecutor> handlerExecutors, IMessage message, CancellationToken cancellationToken);
}