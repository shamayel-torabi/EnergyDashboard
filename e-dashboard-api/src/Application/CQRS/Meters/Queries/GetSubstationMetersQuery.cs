using EnergyDashboard.Application.Interfaces;
using MediatR;

namespace EnergyDashboard.Application.Meters.Querie;

public class GetSubstationMetersQuery : IRequest<IEnumerable<MeterDto>>
{
    public int SubstationId { get; set; }
}

public class GetSubstationMetersQueryHandler : IRequestHandler<GetSubstationMetersQuery, IEnumerable<MeterDto>>
{
    private readonly IMeterEnergyService _meterEnergyService;
    public GetSubstationMetersQueryHandler(IMeterEnergyService meterEnergyService)
    {
        _meterEnergyService = meterEnergyService;
    }

    public async Task<IEnumerable<MeterDto>> Handle(GetSubstationMetersQuery request, CancellationToken cancellationToken)
    {
        var mm = await _meterEnergyService.GetMetersFromCash(cancellationToken);

        var meters = mm
            .Where(w => w.StationId == request.SubstationId)
            .ToList();

        return  meters;
    }
}
