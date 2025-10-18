using Domain.Common;
using MeterService.Domain.Entities;

namespace MeterService.Domain.Events;

public sealed class MeterEnergyAddedEvent : DomainEvent
{
    public MeterEnergyAddedEvent(List<MeterEnergyEntity> items)
    {
        Items = items;
    }

    public List<MeterEnergyEntity> Items { get; }
}
