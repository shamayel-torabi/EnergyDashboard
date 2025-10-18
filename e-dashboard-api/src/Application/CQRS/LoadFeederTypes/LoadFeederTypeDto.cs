using EnergyDashboard.Application.Common.Mappings;
using EnergyDashboard.Domain.Entities;

namespace EnergyDashboard.Application.LoadFeederTypes;

public class LoadFeederTypeDto : IMapFrom<LoadFeederType>
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int Type { get; set; }
}
