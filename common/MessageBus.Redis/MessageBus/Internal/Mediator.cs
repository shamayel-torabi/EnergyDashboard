using System.Collections.Concurrent;

namespace MessageBus.Internal;

internal class Mediator : IMediator
{
    private static readonly ConcurrentDictionary<Type, MessageHandlerWrapper> _messageHandlers = new();

    private readonly IServiceProvider _serviceProvider;
    private readonly IMessagePublisher _publisher;

    public Mediator(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        _publisher = new ForeachAwaitPublisher();
    }

    public Task Publish<TMessage>(TMessage message, CancellationToken cancellationToken = default) where TMessage : IMessage
    {
        if (message == null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        var handler = _messageHandlers.GetOrAdd(message.GetType(), static messageType =>
        {
            var wrapperType = typeof(MessageHandlerWrapperImpl<>).MakeGenericType(messageType);
            var wrapper = Activator.CreateInstance(wrapperType) ?? throw new InvalidOperationException($"Could not create wrapper for type {messageType}");
            return (MessageHandlerWrapper)wrapper;
        });

        return handler.Handle(message, _serviceProvider, PublishCore, cancellationToken);
    }

    internal virtual Task PublishCore(IEnumerable<MessageHandlerExecutor> handlerExecutors, IMessage message, CancellationToken cancellationToken)
        => _publisher.Publish(handlerExecutors, message, cancellationToken);

}
