
using EnergyDashboard.Domain.Entities;
using EnergyDashboard.Domain.Repository;
using EnergyDashboard.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace EnergyDashboard.Infrastructure.Repository;

public class EnergyProfileRepository : Repository<Guid, EnergyProfile>, IEnergyProfileRepository
{
    private readonly PersianCalendar _persianCalendar;

    public EnergyProfileRepository(AppDbContext context) : base(context)
    {
        _persianCalendar = new PersianCalendar();
    }

    public async Task<IEnumerable<EnergyProfile>> GetEquipmentEnergyByDateAsync(Guid equipmentId, DateTimeOffset date, CancellationToken cancellationToken = default)
    {
        var dailyEnergys = await _context.EnergyProfiles.Include(i=> i.Equipment)
            .Where(w => w.RecordDate.Date == date.Date && w.Equipment.Id == equipmentId)
            .ToListAsync(cancellationToken);

        return dailyEnergys;
    }

    public async Task<IEnumerable<EnergyProfile>> GetEquipmentEnergyMonthlyAsync(Guid equipmentId, int year, int month, CancellationToken cancellationToken = default)
    {
        DateTime startDate = _persianCalendar.ToDateTime(year, month, 1, 0, 0, 0, 0, 0);
        DateTime endDate = _persianCalendar.ToDateTime(year, month + 1, 1, 0, 0, 0, 0, 0);

        var dailyEnergys = await _context.EnergyProfiles.Include(i => i.Equipment)
            .Where(w => w.RecordDate >= startDate && w.RecordDate < endDate && w.Equipment.Id == equipmentId)
            .ToListAsync(cancellationToken);

        return dailyEnergys;
    }

    public async Task<IEnumerable<EnergyProfile>> GetEquipmentEnergyProfileIntervalAsync(Guid equipmentId, DateTimeOffset startDate, DateTimeOffset endDate, CancellationToken cancellationToken = default)
    {
        var dailyEnergys = await _context.EnergyProfiles.Include(i => i.Equipment)
            .Where(w => w.RecordDate >= startDate && w.RecordDate < endDate && w.Equipment.Id == equipmentId)
            .ToListAsync(cancellationToken);

        return dailyEnergys;
    }

    public async Task<IEnumerable<EnergyProfile>> GetPowerPlantEnergyAsync(int PowerplantId, DateTimeOffset date, CancellationToken cancellationToken = default)
    {
        var dailyEnergys = await _context.EnergyProfiles.Include(i => i.Equipment)
            .Where(w => w.RecordDate.Date == date.Date && w.Equipment.Type == "Generator" && 
                        (w.Equipment as GeneratorFeederEquipment).PowerplantOperatorId == PowerplantId)
            .ToListAsync(cancellationToken);

        return dailyEnergys;
    }

    public async Task<IEnumerable<EnergyProfile>> GetSubstationEnergyAsync(Guid substationId, DateTimeOffset date, CancellationToken cancellationToken = default)
    {
        var dailyEnergys = await _context.EnergyProfiles.Include(i => i.Equipment).ThenInclude(s=> s.Substation)
            .Where(w => w.RecordDate.Date == date.Date && w.Equipment.SubstationId == substationId)
            .ToListAsync(cancellationToken);

        return dailyEnergys;
    }

}
