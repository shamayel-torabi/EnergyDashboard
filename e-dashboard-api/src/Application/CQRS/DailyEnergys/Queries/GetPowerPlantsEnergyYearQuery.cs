using MediatR;
using EnergyDashboard.Domain.Repository;
using EnergyDashboard.Domain.Entities;

namespace EnergyDashboard.Application.DailyEnergys.Queries;

public class GetPowerPlantsEnergyYearQuery : IRequest<IEnumerable<YearlyPowerPlantEnergy>>
{
    public int Year { get; set; }
}

public class GetPowerPlantsEnergyYearQueryHandler : IRequestHandler<GetPowerPlantsEnergyYearQuery, IEnumerable<YearlyPowerPlantEnergy>>
{
    private readonly IDailyEnergyRepository _dailyEnergyRepository;

    public GetPowerPlantsEnergyYearQueryHandler(IDailyEnergyRepository dailyEnergyRepository)
    {
        _dailyEnergyRepository = dailyEnergyRepository;
    }

    public async Task<IEnumerable<YearlyPowerPlantEnergy>> Handle(GetPowerPlantsEnergyYearQuery request, CancellationToken cancellationToken)
    {
        var dailyEnergys = await _dailyEnergyRepository.GetGeneratorFeedersYearlyEnergyAsync(request.Year, cancellationToken);

        var ret = dailyEnergys
            .GroupBy(g => (g.Equipment as GeneratorFeederEquipment).PowerplantOperator.Name)
            .Select(s => new YearlyPowerPlantEnergy
            {
                PowerPlantName = s.Key,
                ActiveEnergy = s.Sum(a => a.ImportWatt)
            }).OrderBy(o => o.PowerPlantName).ToList();

        return ret;
    }
}

public class YearlyPowerPlantEnergy
{
    public string PowerPlantName { get; set; }
    public decimal ActiveEnergy { get; set; }
}
