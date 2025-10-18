using MeterService.Application.Interfaces;
using MeterService.Application.Models;
using MediatR;
using MeterService.Domain.Common;

namespace MeterService.Infrastructure.Services;

public sealed class DomainEventService : IDomainEventService
{
    private readonly IPublisher _mediator;

    public DomainEventService(IPublisher mediator)
    {
        _mediator = mediator;
    }

    public async Task Publish(DomainEvent domainEvent)
    {
        await _mediator.Publish(GetNotificationCorrespondingToDomainEvent(domainEvent));
    }

    private INotification GetNotificationCorrespondingToDomainEvent(DomainEvent domainEvent)
    {
        return (INotification)Activator.CreateInstance(
            typeof(DomainEventNotification<>).MakeGenericType(domainEvent.GetType()), domainEvent);
    }
}
