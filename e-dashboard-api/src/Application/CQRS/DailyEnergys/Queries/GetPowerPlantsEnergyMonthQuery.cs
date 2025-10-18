using MediatR;
using EnergyDashboard.Domain.Repository;
using EnergyDashboard.Domain.Entities;

namespace EnergyDashboard.Application.DailyEnergys.Queries;

public class GetPowerPlantsEnergyMonthQuery : IRequest<IEnumerable<YearlyPowerPlantEnergy>>
{
    public int Year { get; set; }
    public int Month { get; set; }
}

public class GetPowerPlantsEnergyMonthQueryHandler : IRequestHandler<GetPowerPlantsEnergyMonthQuery, IEnumerable<YearlyPowerPlantEnergy>>
{
    private readonly IDailyEnergyRepository _dailyEnergyRepository;

    public GetPowerPlantsEnergyMonthQueryHandler(IDailyEnergyRepository dailyEnergyRepository)
    {
        _dailyEnergyRepository = dailyEnergyRepository;
    }

    public async Task<IEnumerable<YearlyPowerPlantEnergy>> Handle(GetPowerPlantsEnergyMonthQuery request, CancellationToken cancellationToken)
    {
        var sumEnergyGroup = await _dailyEnergyRepository.GetGeneratorFeedersMonthlyEnergyAsync(request.Year, request.Month, cancellationToken);

        var ret = sumEnergyGroup
            .GroupBy(g => (g.Equipment as GeneratorFeederEquipment).PowerplantOperator.Name)
            .Select(s => new YearlyPowerPlantEnergy
            {
                PowerPlantName = s.Key,
                ActiveEnergy = s.Sum(a => a.ImportWatt)
            })
            .OrderBy(o => o.PowerPlantName).ToList();

        return ret;
    }
}
