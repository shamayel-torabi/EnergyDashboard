using EnergyDashboard.Application.Common.Mappings;
using EnergyDashboard.Domain.Entities;

namespace EnergyDashboard.Application.MeterReadingFailures;

public class MeterReadingFailureDto : IMapFrom<MeterReadingFailure>
{
    public Guid Id { get; set; }
    public DateTime RecordDate { get; set; }
    public Guid EquipmentId { get; set; }
}
