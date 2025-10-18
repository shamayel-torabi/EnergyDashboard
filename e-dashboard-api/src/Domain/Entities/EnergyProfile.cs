
using Domain.Common;

namespace EnergyDashboard.Domain.Entities;

public class EnergyProfile: Entity<Guid>
{
    public EnergyProfile(Guid id) : base(id)
    {

    }

    public DateTimeOffset RecordDate { get; set; }
    public decimal ImportWatt { get; set; }
    public decimal ImportVar { get; set; }
    public decimal ExportWatt { get; set; }
    public decimal ExportVar { get; set; }
    public decimal? ImportTotalWatt { get; set; }
    public decimal? ExportTotalWatt { get; set; }

    public Guid EquipmentId { get; set; }
    public virtual Equipment Equipment { get; set; }
}
