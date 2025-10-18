using MediatR;
using MeterService.gRPC.Services;

namespace EnergyDashboard.Application.MeterEnergys.Commands;

public class DownloadMeterEnergyCommand : IRequest
{
    public DateTimeOffset StartDate { get; set; }
    public DateTimeOffset EndDate { get; set; }
    public string SerialNumber { get; set; }
    public bool Update { get; set; } = false;
}

public class DownloadMeterEnergyCommandHandler : IRequestHandler<DownloadMeterEnergyCommand>
{
    private readonly IMeterClient _meterReaingService;

    public DownloadMeterEnergyCommandHandler(IMeterClient meterReaingService)
    {
        _meterReaingService = meterReaingService;
    }

    public async Task Handle(DownloadMeterEnergyCommand request, CancellationToken cancellationToken)
    {
        await _meterReaingService.DownloadMeterEnergy(request.StartDate, request.EndDate, request.SerialNumber, request.Update);
    }
}
