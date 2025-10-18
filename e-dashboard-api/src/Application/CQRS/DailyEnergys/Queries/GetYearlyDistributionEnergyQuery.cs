using MediatR;
using EnergyDashboard.Domain.Repository;

namespace EnergyDashboard.Application.DailyEnergys.Queries;

public class GetYearlyDistributionEnergyQuery : IRequest<IEnumerable<YearlyLoadEnergy>>
{
    public int Year { get; set; }
}

public class GetDistributionEnergyYearQueryHandler : IRequestHandler<GetYearlyDistributionEnergyQuery, IEnumerable<YearlyLoadEnergy>>
{
    private readonly IDailyEnergyRepository _dailyEnergyRepository;

    public GetDistributionEnergyYearQueryHandler(IDailyEnergyRepository dailyEnergyRepository)
    {
        _dailyEnergyRepository = dailyEnergyRepository;
    }

    public async Task<IEnumerable<YearlyLoadEnergy>> Handle(GetYearlyDistributionEnergyQuery request, CancellationToken cancellationToken)
    {
        var dailyEnergys = await _dailyEnergyRepository.GetLoadFeedersYearlyEnergyAsync(request.Year, 1, cancellationToken);

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

