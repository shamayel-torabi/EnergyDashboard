using EnergyDashboard.Application.Common.Mappings;
using EnergyDashboard.Application.Zones;
using EnergyDashboard.Domain.Entities;

namespace EnergyDashboard.Application.Areas;

public class AreaDto : IMapFrom<Area>
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public IEnumerable<ZoneDto> Zones { get; set; }
}
