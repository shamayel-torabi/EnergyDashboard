
using EnergyDashboard.Domain.Entities;
using EnergyDashboard.Domain.Repository;
using EnergyDashboard.Infrastructure.Persistence;

namespace EnergyDashboard.Infrastructure.Repository;

public class PowerplantOperatorRepository : Repository<int, PowerplantOperator>, IPowerplantOperatorRepository
{
    public PowerplantOperatorRepository(AppDbContext context) : base(context)
    {
    }
}
