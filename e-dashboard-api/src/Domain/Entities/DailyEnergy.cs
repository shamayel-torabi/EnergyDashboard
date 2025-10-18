
using Domain.Common;

namespace EnergyDashboard.Domain.Entities;

public class DailyEnergy : Entity<Guid>
{
    public DailyEnergy(Guid id) : base(id)
    {

    }

    public DateTime RecordDate { get; set; }
    public decimal ImportWatt { get; set; }
    public decimal ImportVar { get; set; }
    public decimal ExportWatt { get; set; }
    public decimal ExportVar { get; set; }

    public Guid EquipmentId { get; set; }
    public virtual Equipment Equipment { get; set;}
}
