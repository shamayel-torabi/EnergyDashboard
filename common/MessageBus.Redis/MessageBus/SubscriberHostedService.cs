using Microsoft.Extensions.Hosting;

namespace MessageBus;

public class SubscriberHostedService : BackgroundService
{
    private readonly IMessageBusProvider _messageBusProvider;

    public SubscriberHostedService(IMessageBusProvider messageBusProvider)
    {
        _messageBusProvider = messageBusProvider ?? throw new ArgumentNullException(nameof(messageBusProvider));
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _messageBusProvider.StartSubscription(stoppingToken);
    }
}