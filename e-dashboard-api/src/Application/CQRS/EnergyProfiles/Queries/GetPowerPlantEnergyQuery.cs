using MediatR;
using EnergyDashboard.Domain.Repository;

namespace EnergyDashboard.Application.EnergyProfiles.Queries;

public class GetPowerPlantEnergyQuery : IRequest<IEnumerable<PowerPlantEnergy>>
{
    public int PowerplantId { get; set; }
    public DateTimeOffset RecordDate { get; set; }
}

public class GetPowerPlantEnergyQueryHandler : IRequestHandler<GetPowerPlantEnergyQuery, IEnumerable<PowerPlantEnergy>>
{
    private readonly IEnergyProfileRepository _energyProfileRepository;

    public GetPowerPlantEnergyQueryHandler(IEnergyProfileRepository energyProfileRepository)
    {
        _energyProfileRepository = energyProfileRepository;
    }

    public async Task<IEnumerable<PowerPlantEnergy>> Handle(GetPowerPlantEnergyQuery request, CancellationToken cancellationToken)
    {
        var energyProfile = await _energyProfileRepository.GetPowerPlantEnergyAsync(request.PowerplantId, request.RecordDate, cancellationToken);

        var ret = energyProfile
            .GroupBy(g => g.Equipment.Name)
            .Select(s => new PowerPlantEnergy
            {
                EquipmentName = s.Key,
                Items = s.Select(x => new EnergyItem
                {
                    RecordDate = x.RecordDate,
                    ImportWatt = x.ImportWatt,
                    ExportWatt = x.ExportWatt,
                    ImportVar = x.ImportVar,
                    ExportVar = x.ExportVar
                }).OrderBy(o => o.RecordDate)
            })
            .OrderBy(o => o.EquipmentName)
            .ToList();

        return ret;
    }
}

public class PowerPlantEnergy
{
    public string EquipmentName { get; set; }
    public IEnumerable<EnergyItem> Items { get; set; }
}
