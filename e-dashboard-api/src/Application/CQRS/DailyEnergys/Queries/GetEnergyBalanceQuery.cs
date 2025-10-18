using MediatR;
using EnergyDashboard.Domain.Repository;

namespace EnergyDashboard.Application.DailyEnergys.Queries;

public class GetEnergyBalanceQuery : IRequest<IEnumerable<EnergyBalance>>
{
    public DateTime RecordDate { get; set; }
}

public class GetEnergyBalanceQueryHandler : IRequestHandler<GetEnergyBalanceQuery, IEnumerable<EnergyBalance>>
{
    private readonly IDailyEnergyRepository _dailyEnergyRepository;

    public GetEnergyBalanceQueryHandler(IDailyEnergyRepository dailyEnergyRepository)
    {
        _dailyEnergyRepository = dailyEnergyRepository;
    }

    public async Task<IEnumerable<EnergyBalance>> Handle(GetEnergyBalanceQuery request, CancellationToken cancellationToken)
    {
        var dailyEnergys = await _dailyEnergyRepository.GetEnergyByDateAsync(request.RecordDate, cancellationToken);

        var energyBalance = dailyEnergys
            .GroupBy(o => o.Equipment.Substation)
            .Select(s => new EnergyBalance
            {
                SubstationId = s.Key.Id,
                SubstationName = s.Key.Name,
                BusBars = s.GroupBy(g => g.Equipment.BusbarName)
                .Select(x => new BasBarEnergyBalance
                {
                    BusBarName = x.Key,
                    ActiveEnergy = x.Sum(ss => ss.ImportWatt - ss.ExportWatt)
                }).OrderBy(o => o.BusBarName)
            }).OrderBy(o => o.SubstationName).ToList();

        return energyBalance;
    }
}


public class BasBarEnergyBalance
{
    public string BusBarName { get; set; }
    public decimal ActiveEnergy { get; set; }
}

public class EnergyBalance
{
    public Guid SubstationId { get; set; }
    public string SubstationName { get; set; }
    public IEnumerable<BasBarEnergyBalance> BusBars { get; set; }
}
