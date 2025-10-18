

using MeterService.Domain.Common;

namespace MeterService.Domain.Entities;

public sealed class MeterEnergyEntity : AuditableEntity, IHasDomainEvent
{
    public Guid MeterEnergyId { get; set; }
    public int MeterId { get; set; }
    public DateTimeOffset RecordDate { get; set; }
    public int? ToolTypeId { get; set; }
    public decimal EnergyActiveExport { get; set; }
    public decimal EnergyActiveImport { get; set; }
    public decimal EnergyReactiveExport { get; set; }
    public decimal EnergyReactiveImport { get; set; }

    public MeterEntity Meter { get; set; }
    public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
}
