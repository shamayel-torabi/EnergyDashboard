using Google.Protobuf.WellKnownTypes;
using MediatR;
using MeterService.gRPC;
using MeterService.gRPC.Services;

namespace EnergyDashboard.Application.Meters.Commands;

public class UpdateMeterCommand : IRequest
{
    public int Id { get; set; }
    public MeterDto Meter { get; set; }   
}

public class UpdateMeterCommandHandler : IRequestHandler<UpdateMeterCommand>
{
    private readonly IMeterClient _meterService;

    public UpdateMeterCommandHandler(IMeterClient meterService)
    {
        _meterService = meterService;
    }

    public async Task Handle(UpdateMeterCommand request, CancellationToken cancellationToken)
    {
        var meter = new Meter()
        { 
            MeterId = request.Meter.MeterId,
            SerialNumber = request.Meter.SerialNumber,
            Name = request.Meter.Name,
            StationId = request.Meter.StationId ?? 0,
            StationName = request.Meter.StationName,
            Active = request.Meter.Active,
            Dismount = request.Meter.Dismount,
            StartOperationDate = request.Meter.StartOperationDate.ToTimestamp(),
            EndOperationDate = request.Meter.EndOperationDate.ToTimestamp(),
        };

        await _meterService.UpdateMeter(meter);
    }
}
