using EnergyDashboard.Application.Interfaces;
using MediatR;

namespace EnergyDashboard.Application.Meters.Querie;

public class GetMeterQuery : IRequest<MeterDto>
{
    public string SerialNumber { get; set; }
}

public class GetMeterQueryHandler : IRequestHandler<GetMeterQuery, MeterDto>
{
    private readonly IMeterEnergyService _meterEnergyService;
    public GetMeterQueryHandler(IMeterEnergyService meterEnergyService)
    {
        _meterEnergyService = meterEnergyService;
    }

    public async Task<MeterDto> Handle(GetMeterQuery request, CancellationToken cancellationToken)
    {
        var meters = await _meterEnergyService.GetMetersFromCash(cancellationToken);
        var meter = meters
            .Where(w => w.SerialNumber == request.SerialNumber)
            .SingleOrDefault();
        return meter;
    }
}
