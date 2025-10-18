using MediatR;
using EnergyDashboard.Domain.Repository;

namespace EnergyDashboard.Application.EnergyProfiles.Queries;

public class GetEquipmentDailyEnergyQuery : IRequest<IEnumerable<EquipmentDailyEnergy>>
{
    public Guid EquipmentId { get; set; }
    public DateTimeOffset RecordDate { get; set; }
}

public class GetEquipmentDailyEnergyQueryHandler : IRequestHandler<GetEquipmentDailyEnergyQuery, IEnumerable<EquipmentDailyEnergy>>
{
    private readonly IEnergyProfileRepository _energyProfileRepository;

    public GetEquipmentDailyEnergyQueryHandler(IEnergyProfileRepository energyProfileRepository)
    {
        _energyProfileRepository = energyProfileRepository;
    }

    public async Task<IEnumerable<EquipmentDailyEnergy>> Handle(GetEquipmentDailyEnergyQuery request, CancellationToken cancellationToken)
    {
        var energyProfile = await _energyProfileRepository.GetEquipmentEnergyByDateAsync(request.EquipmentId, request.RecordDate, cancellationToken);

        var ret = energyProfile
            .GroupBy(g => g.Equipment.Name)
            .Select(s => new EquipmentDailyEnergy
            {
                EquipmentName = s.Key,
                Items = s.Select(x => new DailyEn
                {
                    RecordDate = x.RecordDate,
                    ActiveEnergy = x.ExportWatt - x.ImportWatt
                })
            })
            .OrderBy(o => o.EquipmentName)
            .ToList();

        return ret;
    }
}


public class DailyEn
{
    public DateTimeOffset RecordDate { get; set; }
    public decimal ActiveEnergy { get; set; }
}
public class EquipmentDailyEnergy
{
    public string EquipmentName { get; set; }
    public IEnumerable<DailyEn> Items { get; set; }
}
