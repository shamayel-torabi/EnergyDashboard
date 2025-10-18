using MediatR;
using System.Globalization;
using EnergyDashboard.Domain.Repository;
using EnergyDashboard.Domain.Entities;

namespace EnergyDashboard.Application.DailyEnergys.Queries;

public class GetMonthlyPowerPlantsEnergyQuery : IRequest<IEnumerable<MonthlyPowerPlantsEnergy>>
{
    public int Year { get; set; }
}

public class GetPowerPlantsMonthlyQueryHandler : IRequestHandler<GetMonthlyPowerPlantsEnergyQuery, IEnumerable<MonthlyPowerPlantsEnergy>>
{
    private readonly IDailyEnergyRepository _dailyEnergyRepository;

    public GetPowerPlantsMonthlyQueryHandler(IDailyEnergyRepository dailyEnergyRepository)
    {
        _dailyEnergyRepository = dailyEnergyRepository;
    }

    public async Task<IEnumerable<MonthlyPowerPlantsEnergy>> Handle(GetMonthlyPowerPlantsEnergyQuery request, CancellationToken cancellationToken)
    {
        PersianCalendar pc = new PersianCalendar();

        var dailyEnergys = await _dailyEnergyRepository.GetGeneratorFeedersYearlyEnergyAsync(request.Year, cancellationToken);

        var ret = dailyEnergys
            .GroupBy(g => (g.Equipment as GeneratorFeederEquipment).PowerplantOperator.Name)
            .Select(s => new MonthlyPowerPlantsEnergy
            {
                PowerPlantName = s.Key,
                Energies = s.GroupBy(gg => pc.GetMonth(gg.RecordDate))
                .Select(x => new MonthlyEnergy
                {
                    Month = x.Key,
                    ActiveEnergy = x.Sum(a => a.ImportWatt)
                }).OrderBy(o => o.Month)
            })
            .OrderBy(o => o.PowerPlantName).ToList();

        return ret;
    }
}


public class MonthlyEnergy
{
    public int Month { get; set; }
    public decimal ActiveEnergy { get; set; }
}
public class MonthlyPowerPlantsEnergy
{
    public string PowerPlantName { get; set; }
    public IEnumerable<MonthlyEnergy> Energies { get; set; }
}
