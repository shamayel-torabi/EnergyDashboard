using MediatR;
using System.Globalization;
using EnergyDashboard.Domain.Repository;

namespace EnergyDashboard.Application.DailyEnergys.Queries;

public class GetMonthlyIndustrialEnergyQuery : IRequest<IEnumerable<MonthlyLoadEnergy>>
{
    public int Year { get; set; }
}

public class GetIndustrialLoadMonthlyQueryHandler : IRequestHandler<GetMonthlyIndustrialEnergyQuery, IEnumerable<MonthlyLoadEnergy>>
{
    private readonly IDailyEnergyRepository _dailyEnergyRepository;

    public GetIndustrialLoadMonthlyQueryHandler(IDailyEnergyRepository dailyEnergyRepository)
    {
        _dailyEnergyRepository = dailyEnergyRepository;
    }

    public async Task<IEnumerable<MonthlyLoadEnergy>> Handle(GetMonthlyIndustrialEnergyQuery request, CancellationToken cancellationToken)
    {
        PersianCalendar pc = new PersianCalendar();

        var dailyEnergys = await _dailyEnergyRepository.GetLoadFeedersYearlyEnergyAsync(request.Year, 2, cancellationToken);

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
            }).OrderBy(o => o.Name);

        return sumEnergyGroup;
    }
}


public class MonthlyLoadEnergy
{
    public string Name { get; set; }
    public IEnumerable<MonthlyEnergy> Energies { get; set; }
}
