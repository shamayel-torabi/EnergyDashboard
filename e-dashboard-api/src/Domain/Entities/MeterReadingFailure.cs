
using Domain.Common;

namespace EnergyDashboard.Domain.Entities;

public class MeterReadingFailure : Entity<Guid>
{
    public MeterReadingFailure(Guid id) : base(id)
    {
    }
    public DateTime RecordDate { get; set; }
    public Guid EquipmentId { get; set; }
    public virtual Equipment Equipment { get; set; }
}
