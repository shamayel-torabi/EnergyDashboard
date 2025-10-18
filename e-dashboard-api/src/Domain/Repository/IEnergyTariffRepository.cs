using EnergyDashboard.Domain.Entities;

namespace EnergyDashboard.Domain.Repository;

public interface IEnergyTariffRepository : IRepository<Guid, EnergyTariff>
{
}
