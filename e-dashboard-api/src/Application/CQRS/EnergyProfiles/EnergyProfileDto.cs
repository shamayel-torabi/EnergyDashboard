using EnergyDashboard.Application.Common.Mappings;
using EnergyDashboard.Domain.Entities;

namespace EnergyDashboard.Application.EnergyProfiles;

public class EnergyProfileDto : IMapFrom<EnergyProfile>
{
    public Guid Id { get; set; }
    public DateTimeOffset RecordDate { get; set; }
    public decimal ImportWatt { get; set; }
    public decimal ImportVar { get; set; }
    public decimal ExportWatt { get; set; }
    public decimal ExportVar { get; set; }
    public decimal? ImportTotalWatt { get; set; }
    public decimal? ExportTotalWatt { get; set; }
    public Guid EquipmentId { get; set; }
}
