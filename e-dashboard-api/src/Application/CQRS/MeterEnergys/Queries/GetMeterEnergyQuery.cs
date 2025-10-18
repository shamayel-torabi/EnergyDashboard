using EnergyDashboard.Application.Interfaces;
using EnergyDashboard.Application.Models.Meters;
using MediatR;

namespace EnergyDashboard.Application.MeterEnergys.Queries;

public class GetMeterEnergyQuery : IRequest<IEnumerable<MeterEnergyList>>
{
    public DateTimeOffset RecordDate { get; set; }
}

public class GetMeterEnergyQueryHandler : IRequestHandler<GetMeterEnergyQuery, IEnumerable<MeterEnergyList>>
{
    private readonly IMeterEnergyService _meterEnergyService;

    public GetMeterEnergyQueryHandler(IMeterEnergyService meterEnergyService)
    {
        _meterEnergyService = meterEnergyService;
    }

    public async Task<IEnumerable<MeterEnergyList>> Handle(GetMeterEnergyQuery request, CancellationToken cancellationToken)
    {
        var meterEnergyList = await _meterEnergyService.GetMeterEnergyFromCacheByDate(request.RecordDate.Date, cancellationToken);
        return meterEnergyList;
    }
}
