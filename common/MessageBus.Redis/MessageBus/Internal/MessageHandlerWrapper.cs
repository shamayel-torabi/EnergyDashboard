using Microsoft.Extensions.DependencyInjection;

namespace MessageBus.Internal;

internal abstract class MessageHandlerWrapper
{
    public abstract Task Handle(IMessage message, IServiceProvider serviceFactory,
        Func<IEnumerable<MessageHandlerExecutor>, IMessage, CancellationToken, Task> publish,
        CancellationToken cancellationToken);
}

internal class MessageHandlerWrapperImpl<TMessage> : MessageHandlerWrapper
    where TMessage : IMessage
{
    public override Task Handle(IMessage message, IServiceProvider serviceFactory,
        Func<IEnumerable<MessageHandlerExecutor>, IMessage, CancellationToken, Task> publish,
        CancellationToken cancellationToken)
    {
        var handlers = serviceFactory
            .GetServices<IMessageHandler<TMessage>>()
            .Select(static x => new MessageHandlerExecutor(x, (theMessage, theToken) => x.Handle((TMessage)theMessage, theToken)));

        return publish(handlers, message, cancellationToken);
    }
}