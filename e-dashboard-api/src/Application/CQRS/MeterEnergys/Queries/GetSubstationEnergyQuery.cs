using EnergyDashboard.Application.Models.Meters;
using EnergyDashboard.Application.Interfaces;
using MediatR;

namespace EnergyDashboard.Application.MeterEnergys.Queries;

public class GetSubstationEnergyQuery : IRequest<IEnumerable<SubstationMeterEnergyList>>
{
    public int SubstationId { get; set; }
    public DateTimeOffset RecordDate { get; set; }
}

public class GetSubstationEnergyQueryHandler : IRequestHandler<GetSubstationEnergyQuery, IEnumerable<SubstationMeterEnergyList>>
{
    private readonly IMeterEnergyService _meterEnergyService;
    public GetSubstationEnergyQueryHandler(IMeterEnergyService meterEnergyService)
    {
        _meterEnergyService = meterEnergyService;
    }


    public async Task<IEnumerable<SubstationMeterEnergyList>> Handle(GetSubstationEnergyQuery request, CancellationToken cancellationToken)
    {
        var meterEnergyList = await _meterEnergyService.GetMeterEnergyFromCacheByDate(request.RecordDate.Date, cancellationToken);

        var substationsEnergy = meterEnergyList
            .Where(sub => sub.StationId == request.SubstationId)
            .GroupBy(g => new { g.StationId, g.StationName })
            .Select(s => new SubstationMeterEnergyList
            {
                StationName = s.Key.StationName,
                StationId = s.Key.StationId,
                Equipments = s.Select(x => new EquipmentsList
                {
                    MeterId = x.MeterId,
                    SerialNumber = x.SerialNumber,
                    Name = x.Name,
                    DailyEnergy = x.DailyEnergy,
                    Anomal = x.Anomal,
                })
                .OrderBy(o => o.Name)
                .ToList()
            })
            .ToList();
        return substationsEnergy;
    }
}
