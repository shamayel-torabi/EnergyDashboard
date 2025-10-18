using Domain.Common;

namespace EnergyDashboard.Domain.Entities;

public class LoadFeederType : Entity<int>
{
    public string Name { get; set; }
    public int Type { get; set; }
}
