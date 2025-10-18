using MediatR;
using EnergyDashboard.Domain.Repository;

namespace EnergyDashboard.Application.DailyEnergys.Queries;

public class GetYearlyIndustrialLoadEnergyQuery : IRequest<IEnumerable<YearlyLoadEnergy>>
{
    public int Year { get; set; }
}

public class GetIndustrialLoadEnergyYearQueryHandler : IRequestHandler<GetYearlyIndustrialLoadEnergyQuery, IEnumerable<YearlyLoadEnergy>>
{
    private readonly IDailyEnergyRepository _dailyEnergyRepository;

    public GetIndustrialLoadEnergyYearQueryHandler(IDailyEnergyRepository dailyEnergyRepository)
    {
        _dailyEnergyRepository = dailyEnergyRepository;
    }

    public async Task<IEnumerable<YearlyLoadEnergy>> Handle(GetYearlyIndustrialLoadEnergyQuery request, CancellationToken cancellationToken)
    {
        var dailyEnergys = await _dailyEnergyRepository.GetLoadFeedersYearlyEnergyAsync(request.Year, 2, cancellationToken);

        var sumEnergyGroup = dailyEnergys

            .GroupBy(g => g.Equipment.Name)
            .Select(s => new YearlyLoadEnergy
            {
                Name = s.Key,
                ActiveEnergy = s.Sum(a => a.ExportWatt - a.ImportWatt)
            })
            .OrderBy(o => o.Name).ToList();

        return sumEnergyGroup;
    }
}

public class YearlyLoadEnergy
{
    public string Name { get; set; }
    public decimal ActiveEnergy { get; set; }
}
