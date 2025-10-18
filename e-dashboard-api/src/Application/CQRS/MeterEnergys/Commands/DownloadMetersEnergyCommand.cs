using MediatR;
using MeterService.gRPC.Services;

namespace EnergyDashboard.Application.MeterEnergys.Commands;

public class DownloadMetersEnergyCommand : IRequest
{
    public DateTimeOffset StartDate { get; set; }
    public DateTimeOffset EndDate { get; set; }
    public bool Update { get; set; } = false;
}

public class DownloadMetersEnergyCommandHandler : IRequestHandler<DownloadMetersEnergyCommand>
{
    private readonly IMeterClient _meterReaingService;

    public DownloadMetersEnergyCommandHandler(IMeterClient meterReaingService)
    {
        _meterReaingService = meterReaingService;
    }

    public async Task Handle(DownloadMetersEnergyCommand request, CancellationToken cancellationToken)
    {
        await _meterReaingService.DownloadMetersEnergy(request.StartDate, request.EndDate, request.Update);
    }
}
