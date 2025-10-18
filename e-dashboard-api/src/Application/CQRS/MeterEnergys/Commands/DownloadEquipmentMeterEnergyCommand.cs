using MediatR;
using MeterService.gRPC.Services;
using EnergyDashboard.Domain.Repository;

namespace EnergyDashboard.Application.MeterEnergys.Commands;

public class DownloadEquipmentMeterEnergyCommand : IRequest
{
    public Guid EquipmentId { get; set; }
    public DateTimeOffset StartDate { get; set; }
    public DateTimeOffset EndDate { get; set; }
    public bool Update { get; set; } = false;
}

public class DownloadEquipmentMeterEnergyCommandHandler : IRequestHandler<DownloadEquipmentMeterEnergyCommand>
{
    private readonly IMeterClient _meterReaingService;
    private readonly IEquipmentRepository _equipmentRepository;

    public DownloadEquipmentMeterEnergyCommandHandler(
        IMeterClient meterReaingService,
        IEquipmentRepository equipmentRepository)
    {
        _meterReaingService = meterReaingService;
        _equipmentRepository = equipmentRepository;
    }

    public async Task Handle(DownloadEquipmentMeterEnergyCommand request, CancellationToken cancellationToken)
    {
        var serialNumber = await GetEquimentSerialNumber(request.EquipmentId);
        await _meterReaingService.DownloadMeterEnergy(request.StartDate, request.EndDate, serialNumber, request.Update);
    }

    private async Task<string> GetEquimentSerialNumber(Guid equipmentId)
    {
        var equipment = await _equipmentRepository.GetByIdAsync(equipmentId); 

        if (equipment is not null)
        {
            var em = equipment.GetActiveMeter();
            if(em is not null)
                return em.SerialNumber;
        }            

        return string.Empty;
    }
}
