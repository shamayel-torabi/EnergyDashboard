
using Domain.Common;

namespace EnergyDashboard.Domain.Entities;

public class PowerplantOperator : Entity<int>
{
    public string Name { get; set; }
}
