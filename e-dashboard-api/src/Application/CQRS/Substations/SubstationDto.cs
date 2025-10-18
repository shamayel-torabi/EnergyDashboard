using EnergyDashboard.Application.Common.Mappings;
using EnergyDashboard.Domain.Entities;

namespace EnergyDashboard.Application.Substations;

public class SubstationDto : IMapFrom<Substation>
{
    public Guid Id { get; set; }
    public string Name { get; set; }
}
