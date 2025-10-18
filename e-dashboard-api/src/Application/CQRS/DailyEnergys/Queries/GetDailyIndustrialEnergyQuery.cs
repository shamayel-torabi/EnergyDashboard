using MediatR;
using System.Globalization;
using EnergyDashboard.Domain.Repository;

namespace EnergyDashboard.Application.DailyEnergys.Queries;

public class GetDailyIndustrialEnergyQuery : IRequest<IEnumerable<DayLoadEnergy>>
{
    public int Year { get; set; }
    public int Month { get; set; }
}

public class GetDailyIndustrialEnergyQueryHandler : IRequestHandler<GetDailyIndustrialEnergyQuery, IEnumerable<DayLoadEnergy>>
{
    private readonly IDailyEnergyRepository _dailyEnergyRepository;

    public GetDailyIndustrialEnergyQueryHandler(IDailyEnergyRepository dailyEnergyRepository)
    {
        _dailyEnergyRepository = dailyEnergyRepository;
    }

    public async Task<IEnumerable<DayLoadEnergy>> Handle(GetDailyIndustrialEnergyQuery request, CancellationToken cancellationToken)
    {
        PersianCalendar pc = new PersianCalendar();

        var dailyEnergys = await _dailyEnergyRepository.GetLoadFeedersMonthlyEnergyAsync(request.Year, request.Month, 2, cancellationToken);

        var sumEnergyGroup = dailyEnergys
            .GroupBy(g => g.Equipment.Name)
            .Select(s => new DayLoadEnergy
            {
                Name = s.Key,
                Energies = s.GroupBy(gg => gg.RecordDate).Select(x => new DayEnergy
                {
                    Day = pc.GetDayOfMonth(x.Key),
                    ActiveEnergy = x.Sum(ss => ss.ExportWatt - ss.ImportWatt)
                }).OrderBy(o => o.Day)
            }).OrderBy(o => o.Name).ToList();


        return sumEnergyGroup;
    }
}
