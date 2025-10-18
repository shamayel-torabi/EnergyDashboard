
using MeterService.Domain.Common;

namespace MeterService.Application.Interfaces;

public interface IDomainEventService
{
    Task Publish(DomainEvent domainEvent);
}
