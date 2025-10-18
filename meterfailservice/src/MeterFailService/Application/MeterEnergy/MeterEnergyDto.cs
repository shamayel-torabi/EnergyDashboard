using MeterFailService.Application.Common.Mappings;
using MeterFailService.Domain.Entities;

namespace MeterFailService.Application.MeterEnergy.Queries;

public sealed record MeterEnergyDto : IMapFrom<MeterEnergyEntity>
{
    public Guid Id { get; init; }
    public string SerialNumber { get; init; }
    public string Name { get; init; }
    public string StationName { get; init; }
    public DateOnly RecordDate { get; init; }
    public bool Anomaly { get; init; }
    public List<HourEnergy> HourEnergys { get; init; }
}

public sealed record MeterEnergyUpdateDto
{
    public bool Anomaly { get; set; }
}
