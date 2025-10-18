using System;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using EnergyDashboard.Application.Interfaces;
using Notification.gRPC.Services;
using Notification.gRPC;
using Microsoft.Extensions.Logging;

namespace EnergyDashboard.Application.EnergyProfiles.Commands;

public class UpdateEquipmentsEnergyProfileCommand : IRequest
{
    public DateTimeOffset StartDate { get; set; }   
    public DateTimeOffset EndDate { get; set; }
    public bool UpdateRecords { get; set; }
}

public class UpdateEquipmentsEnergyProfileCommanddHandler : IRequestHandler<UpdateEquipmentsEnergyProfileCommand>
{
    private readonly IEquipmentEnergyService _equipmentEnergyService;
    private readonly IMessageService _messageService;
    private readonly ILogger<UpdateEquipmentsEnergyProfileCommanddHandler> _logger;

    public UpdateEquipmentsEnergyProfileCommanddHandler(
        IEquipmentEnergyService equipmentEnergyService,
        IMessageService messageService,
        ILogger<UpdateEquipmentsEnergyProfileCommanddHandler> logger)
    {
        _equipmentEnergyService = equipmentEnergyService;
        _messageService = messageService;
        _logger = logger;
    }

    public async Task Handle(UpdateEquipmentsEnergyProfileCommand request, CancellationToken cancellationToken)
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

            await _equipmentEnergyService.UpdateEquipmentsEnergy(date, request.UpdateRecords);
            await _equipmentEnergyService.UpdateEquipmentsDailyEnergy(date, request.UpdateRecords);
            await _messageService.SendProgressAsync(100 * day / days, MessageScope.UpdateEquipmentEnergyTable);

            date = date.AddDays(1);
            day++;
        }
    }
}
