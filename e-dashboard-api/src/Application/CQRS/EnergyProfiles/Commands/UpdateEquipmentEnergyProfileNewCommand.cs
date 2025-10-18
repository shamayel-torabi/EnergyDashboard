using System;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using EnergyDashboard.Application.Interfaces;

namespace EnergyDashboard.Application.EnergyProfiles.Commands;

public class UpdateEquipmentEnergyProfileNewCommand : IRequest
{
    public Guid EquipmentId { get; set; }
    public DateTimeOffset RecordDate { get; set; }
}

public class UpdateEquipmentEnergyProfileNewCommandHandler : IRequestHandler<UpdateEquipmentEnergyProfileNewCommand>
{
    private readonly IEquipmentEnergyService _equipmentEnergyService;

    public UpdateEquipmentEnergyProfileNewCommandHandler(
        IEquipmentEnergyService meterReaingService)

    {
        _equipmentEnergyService = meterReaingService;
    }

    public async Task Handle(UpdateEquipmentEnergyProfileNewCommand request, CancellationToken cancellationToken)
    {
        await _equipmentEnergyService.UpdateEquipmentEnergy(request.EquipmentId, request.RecordDate.Date, true);
    }
}
