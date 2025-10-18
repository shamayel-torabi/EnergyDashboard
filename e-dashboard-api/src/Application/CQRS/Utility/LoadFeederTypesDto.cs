using EnergyDashboard.Domain.Entities;

namespace EnergyDashboard.Application.Utility;

public class LoadFeederTypesDto
{
    public int Value { get; set; }
    public string Title { get; set; }
    public IGrouping<int, LoadFeederType> Items { get; set; }
}
