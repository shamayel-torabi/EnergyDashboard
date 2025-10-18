using MediatR;
using EnergyDashboard.Domain.Repository;

namespace EnergyDashboard.Application.EnergyProfiles.Queries;

public class GetUnitEnergyProfileHourQuery : IRequest<IEnumerable<EquipmentEnergyProfile>>
{
    public Guid EquipmentId { get; set; }
    public int Year { get; set; }
    public int Month { get; set; }
}

public class GetUnitEnergyProfileHourQueryHandler : IRequestHandler<GetUnitEnergyProfileHourQuery, IEnumerable<EquipmentEnergyProfile>>
{
    private readonly IEnergyProfileRepository _energyProfileRepository;

    public GetUnitEnergyProfileHourQueryHandler(IEnergyProfileRepository energyProfileRepository)
    {
        _energyProfileRepository = energyProfileRepository;
    }

    public async Task<IEnumerable<EquipmentEnergyProfile>> Handle(GetUnitEnergyProfileHourQuery request, CancellationToken cancellationToken)
    {
        var energyProfile = await _energyProfileRepository.GetEquipmentEnergyMonthlyAsync(request.EquipmentId, request.Year, request.Month, cancellationToken);

        var ret = energyProfile
            .GroupBy(g => g.RecordDate.Date)
            .Select(s => new EquipmentEnergyProfile
            {
                Date = s.Key,
                Energies = s.Select(x => new EquipmentEnergyProfileHour
                {
                    Hour = x.RecordDate.Hour,
                    ImportWatt = x.ImportWatt,
                    ExportWatt = x.ExportWatt
                }).OrderBy(oh => oh.Hour)
            }).OrderBy(od => od.Date)
            .ToList();

        return ret;
    }
}
