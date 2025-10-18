using MediatR;
using System.Globalization;
using EnergyDashboard.Domain.Repository;
using EnergyDashboard.Domain.Entities;

namespace EnergyDashboard.Application.DailyEnergys.Queries;

public class GetDailyPowerPlantsEnergyQuery : IRequest<IEnumerable<DayPowerPlantsEnergy>>
{
    public int Year { get; set; }
    public int Month { get; set; }
}

public class GetDailyPowerPlantsEnergyQueryHandler : IRequestHandler<GetDailyPowerPlantsEnergyQuery, IEnumerable<DayPowerPlantsEnergy>>
{
    private readonly IDailyEnergyRepository _dailyEnergyRepository;

    public GetDailyPowerPlantsEnergyQueryHandler(IDailyEnergyRepository dailyEnergyRepository)
    {
        _dailyEnergyRepository = dailyEnergyRepository;
    }

    public async Task<IEnumerable<DayPowerPlantsEnergy>> Handle(GetDailyPowerPlantsEnergyQuery request, CancellationToken cancellationToken)
    {
        PersianCalendar pc = new PersianCalendar();

        var sumEnergyGroup = await _dailyEnergyRepository.GetGeneratorFeedersMonthlyEnergyAsync(request.Year, request.Month, cancellationToken);

        var ret = sumEnergyGroup
            .GroupBy(g => (g.Equipment as GeneratorFeederEquipment).PowerplantOperator.Name)
            .Select(s => new DayPowerPlantsEnergy
            {
                PowerPlantName = s.Key,
                Energies = s.GroupBy(gg => gg.RecordDate)
                .Select(x => new DayEnergy
                {
                    Day = pc.GetDayOfMonth(x.Key),
                    ActiveEnergy = x.Sum(ss => ss.ImportWatt)
                }).OrderBy(o => o.Day)
            })
            .OrderBy(o => o.PowerPlantName).ToList();


        return ret;
    }
}


public class DayEnergy
{
    public int Day { get; set; }
    public decimal ActiveEnergy { get; set; }
}

public class DayPowerPlantsEnergy
{
    public string PowerPlantName { get; set; }
    public IEnumerable<DayEnergy> Energies { get; set; }
}
