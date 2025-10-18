using EnergyDashboard.Domain.Entities;
using EnergyDashboard.Domain.Repository;
using EnergyDashboard.Infrastructure.Persistence;

namespace EnergyDashboard.Infrastructure.Repository;

public class PowerplantTypeRepository : Repository<int, PowerplantType>, IPowerplantTypeRepository
{
    public PowerplantTypeRepository(AppDbContext context) : base(context)
    {
    }
}
