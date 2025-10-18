
using EnergyDashboard.Application.Common.Mappings;
using EnergyDashboard.Domain.Entities;

namespace EnergyDashboard.Application.PowerplantTypes;

public class PowerplantTypeDto : IMapFrom<PowerplantType>
{
    public int Id { get; set; }
    public string Name { get; set; }
}
