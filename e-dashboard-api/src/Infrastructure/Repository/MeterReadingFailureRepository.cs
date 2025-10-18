using EnergyDashboard.Domain.Entities;
using EnergyDashboard.Domain.Repository;
using EnergyDashboard.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EnergyDashboard.Infrastructure.Repository;

public class MeterReadingFailureRepository : Repository<Guid, MeterReadingFailure>, IMeterReadingFailureRepository
{
    public MeterReadingFailureRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<MeterReadingFailure>> GetMeterReadingFailuresWithEquipment(CancellationToken cancellationToken = default)
    {
        return await _context.MeterReadingFailures
            .Include(i => i.Equipment)
            .ThenInclude(s => s.Substation)
            .ToListAsync(cancellationToken);
    }
}
