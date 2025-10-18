using MediatR;
using System.Globalization;
using EnergyDashboard.Domain.Repository;
using EnergyDashboard.Domain.Entities;

namespace EnergyDashboard.Application.DailyEnergys.Queries;

public class GetMonthlyUnitEnergyEnergyQuery : IRequest<IEnumerable<MonthlyPowerPlantUnitEnergy>>
{
    public int Year { get; set; }
}

public class GetUnitMonthlyEnergyQueryHandler : IRequestHandler<GetMonthlyUnitEnergyEnergyQuery, IEnumerable<MonthlyPowerPlantUnitEnergy>>
{
    private readonly IDailyEnergyRepository _dailyEnergyRepository;

    public GetUnitMonthlyEnergyQueryHandler(IDailyEnergyRepository dailyEnergyRepository)
    {
        _dailyEnergyRepository = dailyEnergyRepository;
    }

    public async Task<IEnumerable<MonthlyPowerPlantUnitEnergy>> Handle(GetMonthlyUnitEnergyEnergyQuery request, CancellationToken cancellationToken)
    {
        PersianCalendar pc = new PersianCalendar();

        var dailyEnergys = await _dailyEnergyRepository.GetGeneratorFeedersYearlyEnergyAsync(request.Year, cancellationToken);

        var ret = dailyEnergys
            .GroupBy(g => (g.Equipment as GeneratorFeederEquipment).PowerplantOperator.Name)
            .Select(s => new MonthlyPowerPlantUnitEnergy
            {
                PowerPlantName = s.Key,
                Units = s.GroupBy(n => n.Equipment.Name).Select(u => new UnitMonthlyEnergy
                {
                    UnitName = u.Key,
                    Energies = u.GroupBy(m => pc.GetMonth(m.RecordDate))
                    .Select(x => new MonthlyEnergy
                    {
                        Month = x.Key,
                        ActiveEnergy = x.Sum(a => a.ImportWatt)
                    }).OrderBy(om => om.Month)
                }).OrderBy(o => o.UnitName)
            })
            .OrderBy(o => o.PowerPlantName).ToList();


        return ret;
    }
}

public class UnitMonthlyEnergy
{
    public string UnitName { get; set; }
    public IEnumerable<MonthlyEnergy> Energies { get; set; }
}
public class MonthlyPowerPlantUnitEnergy
{
    public string PowerPlantName { get; set; }
    public IEnumerable<UnitMonthlyEnergy> Units { get; set; }
}
