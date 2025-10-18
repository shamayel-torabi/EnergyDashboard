namespace MessageBus.Internal;

internal record MessageHandlerExecutor(object HandlerInstance, Func<IMessage, CancellationToken, Task> HandlerCallback);