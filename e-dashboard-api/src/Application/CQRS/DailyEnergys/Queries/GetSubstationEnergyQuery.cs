using MediatR;
using EnergyDashboard.Domain.Repository;

namespace EnergyDashboard.Application.DailyEnergys.Queries;

public class GetSubstationEnergyQuery : IRequest<IEnumerable<SubstationEnergy>>
{
    public Guid SubstationId { get; set; }
    public DateTime RecordDate { get; set; }
}
public class GetSubstationEnergyQueryHandler : IRequestHandler<GetSubstationEnergyQuery, IEnumerable<SubstationEnergy>>
{
    private readonly IDailyEnergyRepository _dailyEnergyRepository;

    public GetSubstationEnergyQueryHandler(IDailyEnergyRepository dailyEnergyRepository)
    {
        _dailyEnergyRepository = dailyEnergyRepository;
    }

    public async Task<IEnumerable<SubstationEnergy>> Handle(GetSubstationEnergyQuery request, CancellationToken cancellationToken)
    {
        var dailyEnergys = await _dailyEnergyRepository.GetSubstationEnergyByDateAsync(request.SubstationId, request.RecordDate, cancellationToken);

        var ret = dailyEnergys.Select(s => new SubstationEnergy
        {
            EquipmentName = s.Equipment.Name,
            RecordDate = s.RecordDate,
            ImportWatt = s.ImportWatt,
            ExportWatt = s.ExportWatt,
            ImportVar = s.ImportVar,
            ExportVar = s.ExportVar
        }).OrderBy(o => o.EquipmentName).ToList();

        return ret;
    }
}

public class SubstationEnergy
{
    public string EquipmentName { get; set; }
    public DateTime RecordDate { get; set; }
    public decimal ImportWatt { get; set; }
    public decimal ExportWatt { get; set; }
    public decimal ImportVar { get; set; }
    public decimal ExportVar { get; set; }
}
