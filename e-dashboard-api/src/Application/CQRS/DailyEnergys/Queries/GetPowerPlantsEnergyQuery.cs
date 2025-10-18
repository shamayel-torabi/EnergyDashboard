using MediatR;
using EnergyDashboard.Domain.Repository;
using EnergyDashboard.Domain.Entities;

namespace EnergyDashboard.Application.DailyEnergys.Queries;

public class GetPowerPlantsEnergyQuery : IRequest<IEnumerable<YearlyPowerPlantEnergy>>
{
    public DateTime RecordDate { get; set; }
}

public class GetPowerPlantsEnergyQueryHandler : IRequestHandler<GetPowerPlantsEnergyQuery, IEnumerable<YearlyPowerPlantEnergy>>
{
    private readonly IDailyEnergyRepository _dailyEnergyRepository;

    public GetPowerPlantsEnergyQueryHandler(IDailyEnergyRepository dailyEnergyRepository)
    {
        _dailyEnergyRepository = dailyEnergyRepository;
    }

    public async Task<IEnumerable<YearlyPowerPlantEnergy>> Handle(GetPowerPlantsEnergyQuery request, CancellationToken cancellationToken)
    {
        var dailyEnergys = await _dailyEnergyRepository.GetGeneratorFeedersByDateEnergyAsync(request.RecordDate, cancellationToken);

        var ret = dailyEnergys
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
