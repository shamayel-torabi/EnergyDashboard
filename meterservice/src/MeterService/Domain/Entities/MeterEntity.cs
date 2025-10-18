

using MeterService.Domain.Common;

namespace MeterService.Domain.Entities;

public sealed class MeterEntity : AuditableEntity
{
    public int MeterId { get; set; }
    public string SerialNumber { get; set; }
    public string Name { get; set; }
    public int? StationId { get; set; }
    public string StationName { get; set; }
    public bool Active { get; set; } = false;
    public bool Dismount { get; set; } = false;
    public DateTimeOffset StartOperationDate { get; set; }
    public DateTimeOffset EndOperationDate { get; set; }

    public IList<MeterEnergyEntity> MeterEnergys { get; private set; } = new List<MeterEnergyEntity>();
}
