using EnergyDashboard.Application.Common.Mappings;
using EnergyDashboard.Application.Areas;
using EnergyDashboard.Domain.Entities;

namespace EnergyDashboard.Application.Networks;

public class NetworkDto : IMapFrom<Network>
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public IEnumerable<AreaDto> Areas { get; set; }
}
