using EnergyDashboard.Application.Common.Mappings;
using EnergyDashboard.Domain.Entities;

namespace EnergyDashboard.Application.EnergyTariffs;

public class EnergyTariffDto : IMapFrom<EnergyTariff>
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    public IEnumerable<EnergyTariffRate> EnergyTariffRates { get; set; }
}
