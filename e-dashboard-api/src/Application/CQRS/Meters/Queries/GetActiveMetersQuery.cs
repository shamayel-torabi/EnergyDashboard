using EnergyDashboard.Application.Models.Meters;
using EnergyDashboard.Application.Interfaces;
using MediatR;

namespace EnergyDashboard.Application.Meters.Querie;

public class GetActiveMetersQuery : IRequest<IEnumerable<MeterEntity>>
{
}

public class GetActiveMetersQueryHandler : IRequestHandler<GetActiveMetersQuery, IEnumerable<MeterEntity>>
{
    private readonly IMeterEnergyService _meterEnergyService;
    public GetActiveMetersQueryHandler(IMeterEnergyService meterEnergyService)
    {
        _meterEnergyService = meterEnergyService;
    }

    public async Task<IEnumerable<MeterEntity>> Handle(GetActiveMetersQuery request, CancellationToken cancellationToken)
    {
        var meters = await _meterEnergyService.GetMetersFromCash(cancellationToken);
        return meters
            .Where(x => x.Dismount == false)
            .Select(s => new MeterEntity { 
                meterId = s.MeterId,
                serialNumber = s.SerialNumber,
                active = s.Dismount,
                startOperationDate = s.StartOperationDate,
                endOperationDate = s.EndOperationDate 
            })
            .ToList();
    }
}
