using EnergyDashboard.Application.Common.Mappings;
using EnergyDashboard.Domain.Entities;

namespace EnergyDashboard.Application.PowerplantOperators;

public class PowerplantOperatorDto : IMapFrom<PowerplantOperator>
{
    public int Id { get; set; }
    public string Name { get; set; }
}
