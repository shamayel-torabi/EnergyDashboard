using MediatR;
using EnergyDashboard.Domain.Repository;
using EnergyDashboard.Domain.Entities;

namespace EnergyDashboard.Application.DailyEnergys.Queries;

public class GetYearlyUnitEnergyQuery : IRequest<IEnumerable<YearlyUnitEnergy>>
{
    public int Year { get; set; }
}
public class GetUnitEnergyYearQueryHandler : IRequestHandler<GetYearlyUnitEnergyQuery, IEnumerable<YearlyUnitEnergy>>
{
    private readonly IDailyEnergyRepository _dailyEnergyRepository;

    public GetUnitEnergyYearQueryHandler(IDailyEnergyRepository dailyEnergyRepository)
    {
        _dailyEnergyRepository = dailyEnergyRepository;
    }

    public async Task<IEnumerable<YearlyUnitEnergy>> Handle(GetYearlyUnitEnergyQuery request, CancellationToken cancellationToken)
    {
        var powerplantFeeders = await _dailyEnergyRepository.GetGeneratorFeedersYearlyEnergyAsync(request.Year, cancellationToken);

        var ret = powerplantFeeders
            .GroupBy(g => (g.Equipment as GeneratorFeederEquipment).PowerplantOperator.Name)
            .Select(s => new YearlyUnitEnergy
            {
                PowerPlantName = s.Key,
                Units = s.GroupBy(gg => gg.Equipment.Name).Select(x => new UnitEnergy
                {
                    UnitName = x.Key,
                    ActiveEnergy = x.Sum(a => a.ImportWatt),
                }).OrderBy(o => o.UnitName)
            })
            .OrderBy(o => o.PowerPlantName).ToList();

        return ret;
    }
}

public class UnitEnergy
{
    public string UnitName { get; set; }
    public decimal ActiveEnergy { get; set; }
}
public class YearlyUnitEnergy
{
    public string PowerPlantName { get; set; }

    public IEnumerable<UnitEnergy> Units { get; set; }
}
