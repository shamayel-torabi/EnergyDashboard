using MediatR;
using MeterService.gRPC.Services;

namespace EnergyDashboard.Application.Meters.Commands;

public class DeleteMeterCommand : IRequest
{
    public int Id { get; set; }
}

public class DeleteMeterCommandHandler : IRequestHandler<DeleteMeterCommand>
{
    private readonly IMeterClient _meterService;
    public DeleteMeterCommandHandler(IMeterClient meterService)
    {
        _meterService = meterService;
    }

    public async Task Handle(DeleteMeterCommand request, CancellationToken cancellationToken)
    {
        await _meterService.DeleteMeter(request.Id);
    }
}
