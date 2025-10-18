
using EnergyDashboard.Domain.Entities;
using EnergyDashboard.Domain.Repository;
using EnergyDashboard.Infrastructure.Persistence;

namespace EnergyDashboard.Infrastructure.Repository;

public class LoadFeederTypeRepository : Repository<int, LoadFeederType>, ILoadFeederTypeRepository
{
    public LoadFeederTypeRepository(AppDbContext context) : base(context)
    {
    }
}
