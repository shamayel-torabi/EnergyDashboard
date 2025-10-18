using MediatR;
using System.Globalization;
using EnergyDashboard.Domain.Repository;

namespace EnergyDashboard.Application.DailyEnergys.Queries;

public class GetMonthlyDistributionEnergyQuery : IRequest<IEnumerable<MonthlyLoadEnergy>>
{
    public int Year { get; set; }
}

public class GetMonthlyDistributionQueryHandler : IRequestHandler<GetMonthlyDistributionEnergyQuery, IEnumerable<MonthlyLoadEnergy>>
{
    private readonly IDailyEnergyRepository _dailyEnergyRepository;

    public GetMonthlyDistributionQueryHandler(IDailyEnergyRepository dailyEnergyRepository)
    {
        _dailyEnergyRepository = dailyEnergyRepository;
    }

    public async Task<IEnumerable<MonthlyLoadEnergy>> Handle(GetMonthlyDistributionEnergyQuery request, CancellationToken cancellationToken)
    {
        PersianCalendar pc = new PersianCalendar();

        var dailyEnergys = await _dailyEnergyRepository.GetLoadFeedersYearlyEnergyAsync(request.Year, 1, cancellationToken);

        var sumEnergyGroup = dailyEnergys
            .GroupBy(g => g.Equipment.Name)
            .Select(s => new MonthlyLoadEnergy
            {
                Name = s.Key,
                Energies = s.GroupBy(gg => pc.GetMonth(gg.RecordDate))
                .Select(x => new MonthlyEnergy
                {
                    Month = x.Key,
                    ActiveEnergy = x.Sum(a => a.ExportWatt - a.ImportWatt)
                }).OrderBy(o => o.Month)
            }).OrderBy(o => o.Name).ToList();

        return sumEnergyGroup;
    }
}
