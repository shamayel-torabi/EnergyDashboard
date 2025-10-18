using MediatR;
using EnergyDashboard.Application.Interfaces;
using Microsoft.Extensions.Logging;
using Notification.gRPC.Services;
using Notification.gRPC;

namespace EnergyDashboard.Application.EnergyProfiles.Commands;

public class UpdateSubstationEquipmentsEnergyCommand : IRequest
{
    public Guid SubstationId { get; set; }
    public DateTimeOffset StartDate { get; set; }
    public DateTimeOffset EndDate { get; set; }
    public bool UpdateRecords { get; set; }
}

public class UpdateSubstationEquipmentsEnergyCommandHandler : IRequestHandler<UpdateSubstationEquipmentsEnergyCommand>
{
    private readonly IEquipmentEnergyService _equipmentEnergyService;
    private readonly IMessageService _messageService;
    private readonly ILogger<UpdateSubstationEquipmentsEnergyCommandHandler> _logger;

    public UpdateSubstationEquipmentsEnergyCommandHandler(
        IEquipmentEnergyService equipmentEnergyService,
        IMessageService messageService,
        ILogger<UpdateSubstationEquipmentsEnergyCommandHandler> logger)

    {
        _equipmentEnergyService = equipmentEnergyService;
        _messageService = messageService;
        _logger = logger;
    }

    public async Task Handle(UpdateSubstationEquipmentsEnergyCommand request, CancellationToken cancellationToken)
    {
        var days = request.EndDate.Subtract(request.StartDate).Days + 1;
        var day = 1;
        var date = request.StartDate.Date;

        while (date <= request.EndDate)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                _logger.LogInformation("عملیات به روز رسانی جدول پروفایل و انرژی روزانه متوقف شد.");
                break;
            }

            await _equipmentEnergyService.UpdateSubstationEquipmentEnergy(request.SubstationId, date, request.UpdateRecords);
            await _equipmentEnergyService.UpdateSubstationDailyEnergy(request.SubstationId, date, request.UpdateRecords);

            await _messageService.SendProgressAsync(100 * day / days, MessageScope.UpdateEquipmentEnergyTable);

            date = date.AddDays(1);
            day++;
        }
    }
}
