using MeterService.Application.Common.Mappings;
using MeterService.Domain.Entities;

namespace MeterService.Application.MeterEnergy.Queries;

public sealed record MeterEnergyDTO : IMapFrom<MeterEnergyEntity>
{
    public Guid MeterEnergyId { get; init; }
    public int MeterId { get; init; }
    public DateTimeOffset RecordDate { get; init; }
    public int? ToolTypeId { get; init; }
    public decimal EnergyActiveExport { get; init; }
    public decimal EnergyActiveImport { get; init;  }
    public decimal EnergyReactiveExport { get; init; }
    public decimal EnergyReactiveImport { get; init; }
}
