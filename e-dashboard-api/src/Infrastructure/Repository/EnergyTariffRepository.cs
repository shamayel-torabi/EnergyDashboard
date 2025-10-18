
using EnergyDashboard.Domain.Entities;
using EnergyDashboard.Domain.Repository;
using EnergyDashboard.Infrastructure.Persistence;

namespace EnergyDashboard.Infrastructure.Repository;

public class EnergyTariffRepository : Repository<Guid, EnergyTariff>, IEnergyTariffRepository
{
    public EnergyTariffRepository(AppDbContext context) : base(context)
    {
    }
}
