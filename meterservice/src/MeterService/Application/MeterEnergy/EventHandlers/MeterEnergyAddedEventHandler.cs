using MeterService.Application.Models;
using MeterService.Domain.Events;
using MediatR;

namespace MeterService.Application.MeterEnergy.EventHandlers;

public sealed class MeterEnergyAddedEventHandler : INotificationHandler<DomainEventNotification<MeterEnergyAddedEvent>>
{
    private readonly ILogger<MeterEnergyAddedEventHandler> _logger;

    public MeterEnergyAddedEventHandler(ILogger<MeterEnergyAddedEventHandler> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task Handle(DomainEventNotification<MeterEnergyAddedEvent> notification, CancellationToken cancellationToken)
    {
        var domainEvent = notification.DomainEvent;

        _logger.LogInformation($"Published {domainEvent.GetType().Name}: Number Of MetersEnergy Reads:{domainEvent.Items.Count}");
        await Task.Delay(1);
        return ;
    }
}
