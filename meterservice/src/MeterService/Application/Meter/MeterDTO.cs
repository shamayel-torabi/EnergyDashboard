using MeterService.Application.Common.Mappings;
using MeterService.Domain.Entities;

namespace MeterService.Application.Meter;

public sealed record MeterDTO : IMapFrom<MeterEntity>
{
    public int MeterId { get; init; }
    public string SerialNumber { get; init; }
    public string Name { get; init; }
    public int? StationId { get; init; }
    public string StationName { get; init; }
    public bool Active { get; init; } = false;
    public bool Dismount { get; init; } = false;
    public DateTimeOffset StartOperationDate { get; init; }
    public DateTimeOffset EndOperationDate { get; init; }
}
