namespace MessageBus.Internal;

internal class ForeachAwaitPublisher : IMessagePublisher
{
    public async Task Publish(IEnumerable<MessageHandlerExecutor> handlerExecutors, IMessage message, CancellationToken cancellationToken)
    {
        foreach (var handler in handlerExecutors)
        {
            await handler.HandlerCallback(message, cancellationToken).ConfigureAwait(false);
        }
    }
}