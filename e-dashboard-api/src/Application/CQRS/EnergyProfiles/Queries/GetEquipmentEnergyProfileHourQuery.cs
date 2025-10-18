using MediatR;
using EnergyDashboard.Domain.Repository;

namespace EnergyDashboard.Application.EnergyProfiles.Queries;

public class GetEquipmentEnergyProfileHourQuery : IRequest<IEnumerable<EquipmentEnergyProfile>>
{
    public Guid EquipmentId { get; set; }
    public int Year { get; set; }
    public int Month { get; set; }
}

public class GetEquipmentEnergyProfileHourQueryHandler : IRequestHandler<GetEquipmentEnergyProfileHourQuery, IEnumerable<EquipmentEnergyProfile>>
{
    private readonly IEnergyProfileRepository _energyProfileRepository;

    public GetEquipmentEnergyProfileHourQueryHandler(IEnergyProfileRepository energyProfileRepository)
    {
        _energyProfileRepository = energyProfileRepository;
    }

    public async Task<IEnumerable<EquipmentEnergyProfile>> Handle(GetEquipmentEnergyProfileHourQuery request, CancellationToken cancellationToken)
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

public class EquipmentEnergyProfileHour
{
    public int Hour { get; set; }
    public decimal ImportWatt { get; set; }
    public decimal ExportWatt { get; set; }
}

public class EquipmentEnergyProfile
{
    public DateTime Date { get; set; }
    public IEnumerable<EquipmentEnergyProfileHour> Energies { get; set; }
}

