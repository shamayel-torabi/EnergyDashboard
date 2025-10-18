using EnergyDashboard.Domain.Repository;
using MediatR;

namespace EnergyDashboard.Application.EnergyProfiles.Queries;

public class GetSubstationEnergyQuery : IRequest<IEnumerable<SubstationEnergy>>
{
    public Guid SubstationId { get; set; }
    public DateTimeOffset RecordDate { get; set; }
}

public class GetSubstationEnergyQueryHandler : IRequestHandler<GetSubstationEnergyQuery, IEnumerable<SubstationEnergy>>
{
    private readonly IEnergyProfileRepository _energyProfileRepository;

    public GetSubstationEnergyQueryHandler(IEnergyProfileRepository energyProfileRepository)
    {
        _energyProfileRepository = energyProfileRepository;
    }

    public async Task<IEnumerable<SubstationEnergy>> Handle(GetSubstationEnergyQuery request, CancellationToken cancellationToken)
    {
        var energyProfile = await _energyProfileRepository.GetSubstationEnergyAsync(request.SubstationId, request.RecordDate, cancellationToken);

        var ret = energyProfile.GroupBy(g => new { Name = g.Equipment.Name, Id = g.Equipment.Id })
            .Select(s => new SubstationEnergy
            {
                EquipmentId = s.Key.Id,
                EquipmentName = s.Key.Name,
                Items = s.Select(x => new EnergyItem
                {
                    RecordDate = x.RecordDate,
                    ImportWatt = x.ImportWatt,
                    ExportWatt = x.ExportWatt,
                    ImportVar = x.ImportVar,
                    ExportVar = x.ExportVar

                }).OrderBy(o => o.RecordDate)
            }).OrderBy(o => o.EquipmentName).ToList();

        return ret;
    }
}


public class EnergyItem
{
    public DateTimeOffset RecordDate { get; set; }
    public decimal ImportWatt { get; set; }
    public decimal ExportWatt { get; set; }
    public decimal ImportVar { get; set; }
    public decimal ExportVar { get; set; }
}

public class SubstationEnergy
{
    public Guid EquipmentId { get; set;}
    public string EquipmentName { get; set; }
    public IEnumerable<EnergyItem> Items { get; set; }
}

