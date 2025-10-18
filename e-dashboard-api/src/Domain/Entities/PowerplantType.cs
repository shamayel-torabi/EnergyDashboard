
using Domain.Common;

namespace EnergyDashboard.Domain.Entities;

public class PowerplantType : Entity<int>
{
    public string Name { get; set; }
}
