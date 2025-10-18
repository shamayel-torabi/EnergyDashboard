using EnergyDashboard.Application.Interfaces;
using MediatR;

namespace EnergyDashboard.Application.Meters.Querie;

public class GetMetersQuery : IRequest<IEnumerable<MeterDto>>
{
}

public class GetMetersQueryHandler : IRequestHandler<GetMetersQuery, IEnumerable<MeterDto>>
{
    private readonly IMeterEnergyService _meterEnergyService;
    public GetMetersQueryHandler(IMeterEnergyService meterEnergyService)
    {
        _meterEnergyService = meterEnergyService;
    }

    public async Task<IEnumerable<MeterDto>> Handle(GetMetersQuery request, CancellationToken cancellationToken)
    {
        return await _meterEnergyService.GetMetersFromCash(cancellationToken);
    }
}
