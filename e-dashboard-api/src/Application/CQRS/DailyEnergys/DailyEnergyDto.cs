using EnergyDashboard.Application.Common.Mappings;
using EnergyDashboard.Domain.Entities;

namespace EnergyDashboard.Application.DailyEnergys;

public class DailyEnergyDto : IMapFrom<DailyEnergy>
{
    public Guid Id { get; set; }
    public DateTime RecordDate { get; set; }
    public decimal ImportWatt { get; set; }
    public decimal ImportVar { get; set; }
    public decimal ExportWatt { get; set; }
    public decimal ExportVar { get; set; }
    public Guid EquipmentId { get; set; }
}
