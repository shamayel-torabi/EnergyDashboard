using MediatR;
using System.Globalization;
using EnergyDashboard.Domain.Repository;

namespace EnergyDashboard.Application.DailyEnergys.Queries;

public class GetDailyDistributionEnergyQuery : IRequest<IEnumerable<DayLoadEnergy>>
{
    public int Year { get; set; }
    public int Month { get; set; }
}

public class GetDailyDistributionEnergyQueryHandler : IRequestHandler<GetDailyDistributionEnergyQuery, IEnumerable<DayLoadEnergy>>
{
    private readonly IDailyEnergyRepository _dailyEnergyRepository;

    public GetDailyDistributionEnergyQueryHandler(IDailyEnergyRepository dailyEnergyRepository)
    {
        _dailyEnergyRepository = dailyEnergyRepository;
    }

    public async Task<IEnumerable<DayLoadEnergy>> Handle(GetDailyDistributionEnergyQuery request, CancellationToken cancellationToken)
    {
        PersianCalendar pc = new PersianCalendar();

        var dailyEnergys = await _dailyEnergyRepository.GetLoadFeedersMonthlyEnergyAsync(request.Year, request.Month, 1, cancellationToken);

        var sumEnergyGroup = dailyEnergys
            .GroupBy(g => g.Equipment.Name)
            .Select(s => new DayLoadEnergy
            {
                Name =s.Key,
                Energies = s.GroupBy(gg => gg.RecordDate).Select(x => new DayEnergy
                {
                    Day = pc.GetDayOfMonth(x.Key),
                    ActiveEnergy = x.Sum(ss => ss.ExportWatt - ss.ImportWatt)
                }).OrderBy(o => o.Day)
            }).OrderBy(o => o.Name).ToList();


        return sumEnergyGroup;
    }
}

public class DayLoadEnergy
{
    public string Name { get; set; }
    public IEnumerable<DayEnergy> Energies { get; set; }
}
