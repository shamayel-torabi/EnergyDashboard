using MediatR;
using System.Globalization;
using EnergyDashboard.Domain.Repository;
using EnergyDashboard.Domain.Entities;

namespace EnergyDashboard.Application.DailyEnergys.Queries;

public class GetDailyUnitEnergyQuery : IRequest<IEnumerable<DayPowerPlantUnitsEnergy>>
{
    public int Year { get; set; }
    public int Month { get; set; }
}

public class GetDailyUnitEnergyQueryHandler : IRequestHandler<GetDailyUnitEnergyQuery, IEnumerable<DayPowerPlantUnitsEnergy>>
{
    private readonly IDailyEnergyRepository _dailyEnergyRepository;

    public GetDailyUnitEnergyQueryHandler(IDailyEnergyRepository dailyEnergyRepository)
    {
        _dailyEnergyRepository = dailyEnergyRepository;
    }

    public async Task<IEnumerable<DayPowerPlantUnitsEnergy>> Handle(GetDailyUnitEnergyQuery request, CancellationToken cancellationToken)
    {
        PersianCalendar pc = new PersianCalendar();

        var sumEnergyGroup = await _dailyEnergyRepository.GetGeneratorFeedersMonthlyEnergyAsync(request.Year, request.Month, cancellationToken);

        var ret = sumEnergyGroup
            .GroupBy(g => (g.Equipment as GeneratorFeederEquipment).PowerplantOperator.Name)
            .Select(s => new DayPowerPlantUnitsEnergy
            {
                PowerPlantName = s.Key,
                Units = s.GroupBy(gg => gg.Equipment.Name).Select(x => new DayUnitsEnergy
                {
                    UnitName = x.Key,
                    Energies = x.Select(x => new DayEnergy
                    {
                        Day = pc.GetDayOfMonth(x.RecordDate),
                        ActiveEnergy = x.ImportWatt
                    }).OrderBy(o => o.Day)
                }).OrderBy(o => o.UnitName)
            })
            .OrderBy(o => o.PowerPlantName);


        return ret;
    }
}


public class DayUnitsEnergy
{
    public string UnitName { get; set; }
    public IEnumerable<DayEnergy> Energies { get; set; }
}

public class DayPowerPlantUnitsEnergy
{
    public string PowerPlantName { get; set; }
    public IEnumerable<DayUnitsEnergy> Units { get; set; }
}
