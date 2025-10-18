using MediatR;
using EnergyDashboard.Domain.Entities;
using EnergyDashboard.Domain.Repository;

namespace EnergyDashboard.Application.DailyEnergys.Queries;

public class GetSubstationEnergyBalanceQuery : IRequest<IEnumerable<SubstationEnergyBalance>>
{
    public Guid SubstationId { get; set; }
    public DateTime RecordDate { get; set; }
}

public class GetSubstationEnergyBalanceQueryHandler : IRequestHandler<GetSubstationEnergyBalanceQuery, IEnumerable<SubstationEnergyBalance>>
{
    private readonly IDailyEnergyRepository _dailyEnergyRepository;

    public GetSubstationEnergyBalanceQueryHandler(IDailyEnergyRepository dailyEnergyRepository)
    {
        _dailyEnergyRepository = dailyEnergyRepository;
    }

    public async Task<IEnumerable<SubstationEnergyBalance>> Handle(GetSubstationEnergyBalanceQuery request, CancellationToken cancellationToken)
    {
        var dailyEnergys = await _dailyEnergyRepository.GetSubstationEnergyByDateAsync(request.SubstationId, request.RecordDate, cancellationToken);

        var ret = dailyEnergys.GroupBy(g => g.Equipment.BusbarName)
             .Select(s => new SubstationEnergyBalance
             {
                 BusbarName = s.Key,
                 Equipments = s.Select(x => new SubstationBasEnergy
                 {
                     Name = x.Equipment.Name,
                     DailyEnergy = x
                 }).OrderBy(o => o.Name)
             }).OrderBy(o => o.BusbarName).ToList();

        return ret;
    }
}

public class SubstationBasEnergy { 
    public string Name { get; set; }
    public DailyEnergy DailyEnergy { get; set; }
}
public class SubstationEnergyBalance
{
    public string BusbarName { get; set;}
    public IEnumerable<SubstationBasEnergy> Equipments { get; set; }
}
