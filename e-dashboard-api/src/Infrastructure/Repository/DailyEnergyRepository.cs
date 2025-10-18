using EnergyDashboard.Domain.Entities;
using EnergyDashboard.Domain.Repository;
using EnergyDashboard.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace EnergyDashboard.Infrastructure.Repository;

public class DailyEnergyRepository : Repository<Guid, DailyEnergy>, IDailyEnergyRepository
{
    private readonly PersianCalendar _persianCalendar;

    public DailyEnergyRepository(AppDbContext context):base(context)
    {
        _persianCalendar = new PersianCalendar();
    }

    public async Task<IEnumerable<DailyEnergy>> GetLoadFeedersMonthlyEnergyAsync(int year, int month, int loadFeederTypeId, CancellationToken cancellationToken = default)
    {
        DateTime startDate = _persianCalendar.ToDateTime(year, month, 1, 0, 0, 0, 0, 0);
        DateTime endDate = _persianCalendar.ToDateTime(year, month + 1, 1, 0, 0, 0, 0, 0);

        var dailyEnergys = await _context.DailyEnergys.Include(i => i.Equipment)
            .Where(w => w.RecordDate >= startDate && w.RecordDate < endDate &&
                        w.Equipment.Type == "LoadFeeder" && (w.Equipment as LoadFeederEquipment).LoadFeederTypeId == loadFeederTypeId)
            .ToListAsync(cancellationToken);


        return dailyEnergys;
    }

    public async Task<IEnumerable<DailyEnergy>> GetLoadFeedersYearlyEnergyAsync(int year, int loadFeederTypeId, CancellationToken cancellationToken = default)
    {
        DateTime startDate = _persianCalendar.ToDateTime(year, 1, 1, 0, 0, 0, 0, 0);
        DateTime endDate = _persianCalendar.ToDateTime(year +1, 1, 1, 0, 0, 0, 0, 0);

        var dailyEnergys = await _context.DailyEnergys.Include(i => i.Equipment)
            .Where(w => w.RecordDate >= startDate && w.RecordDate < endDate &&
                        w.Equipment.Type == "LoadFeeder" && (w.Equipment as LoadFeederEquipment).LoadFeederTypeId == loadFeederTypeId)
            .ToListAsync(cancellationToken);


        return dailyEnergys;
    }

    public async Task<IEnumerable<DailyEnergy>> GetLoadFeedersByDateEnergyAsync(DateTime date, int loadFeederTypeId, CancellationToken cancellationToken = default)
    {
        var dailyEnergys = await _context.DailyEnergys.Include(i => i.Equipment)
            .Where(w => w.RecordDate.Date == date.Date &&
                        w.Equipment.Type == "LoadFeeder" && (w.Equipment as LoadFeederEquipment).LoadFeederTypeId == loadFeederTypeId)
            .ToListAsync(cancellationToken);

        return dailyEnergys;
    }

    public async Task<IEnumerable<DailyEnergy>> GetGeneratorFeedersMonthlyEnergyAsync(int year, int month, CancellationToken cancellationToken = default)
    {
        DateTime startDate = _persianCalendar.ToDateTime(year, month, 1, 0, 0, 0, 0, 0);
        DateTime endDate = _persianCalendar.ToDateTime(year, month + 1, 1, 0, 0, 0, 0, 0);

        var dailyEnergys = await _context.DailyEnergys.Include(i => i.Equipment)
            .Where(w => w.RecordDate >= startDate && w.RecordDate < endDate && w.Equipment.Type == "Generator")
            .ToListAsync(cancellationToken);


        return dailyEnergys;
    }


    public async Task<IEnumerable<DailyEnergy>> GetGeneratorFeedersYearlyEnergyAsync(int year, CancellationToken cancellationToken = default)
    {
        DateTime startDate = _persianCalendar.ToDateTime(year, 1, 1, 0, 0, 0, 0, 0);
        DateTime endDate = _persianCalendar.ToDateTime(year + 1, 1, 1, 0, 0, 0, 0, 0);

        var dailyEnergys = await _context.DailyEnergys.Include(i => i.Equipment)
            .Where(w => w.RecordDate >= startDate && w.RecordDate < endDate && w.Equipment.Type == "Generator")
            .ToListAsync(cancellationToken);

        return dailyEnergys;
    }

    public async Task<IEnumerable<DailyEnergy>> GetGeneratorFeedersByDateEnergyAsync(DateTime date, CancellationToken cancellationToken = default)
    {
        var dailyEnergys = await _context.DailyEnergys.Include(i => i.Equipment)
            .Where(w => w.RecordDate.Date == date.Date && w.Equipment.Type == "Generator")
            .ToListAsync(cancellationToken);

        return dailyEnergys;
    }


    public async Task<IEnumerable<DailyEnergy>> GetEnergyByDateAsync(DateTime date, CancellationToken cancellationToken = default)
    {
        var dailyEnergys = await _context.DailyEnergys.Include(i => i.Equipment).ThenInclude(s => s.Substation)
            .Where(w => w.RecordDate.Date == date.Date)
            .ToListAsync(cancellationToken);

        return dailyEnergys;
    }

    public async Task<IEnumerable<DailyEnergy>> GetSubstationEnergyByDateAsync(Guid substationId , DateTime date, CancellationToken cancellationToken = default)
    {
        var dailyEnergys = await _context.DailyEnergys.Include(i => i.Equipment).ThenInclude(s => s.Substation)
            .Where(w => w.RecordDate.Date == date.Date && w.Equipment.SubstationId == substationId)
            .ToListAsync(cancellationToken);

        return dailyEnergys;
    }
}
