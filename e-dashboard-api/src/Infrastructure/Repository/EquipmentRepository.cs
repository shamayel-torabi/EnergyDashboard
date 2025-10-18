
using EnergyDashboard.Domain.Entities;
using EnergyDashboard.Domain.Repository;
using EnergyDashboard.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EnergyDashboard.Infrastructure.Repository;

public class EquipmentRepository : Repository<Guid, Equipment>, IEquipmentRepository
{
    public EquipmentRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<GeneratorFeederEquipment>> GeneratorFeedersEquipmentAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Equipments.OfType<GeneratorFeederEquipment>().ToListAsync(cancellationToken);
    }
}
