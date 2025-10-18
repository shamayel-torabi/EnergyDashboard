using EnergyDashboard.Domain.Entities;

namespace EnergyDashboard.Infrastructure.Persistence;

public class SeedConfig
{
    public List<LoadFeederType> LoadFeederTypes { get; set; }
    public List<PowerplantOperator> PowerplantOperators { get; set; }
    public List<PowerplantType> PowerplantTypes { get; set; }
}
