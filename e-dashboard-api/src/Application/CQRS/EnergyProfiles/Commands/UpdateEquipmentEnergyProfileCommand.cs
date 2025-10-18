using System;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using EnergyDashboard.Application.Interfaces;
using Microsoft.Extensions.Logging;
using Notification.gRPC.Services;
using Notification.gRPC;

namespace EnergyDashboard.Application.EnergyProfiles.Commands;

public class UpdateEquipmentEnergyProfileCommand : IRequest
{
    public Guid EquipmentId { get; set; }
    public DateTimeOffset StartDate { get; set; }
    public DateTimeOffset EndDate { get; set; }
    public bool UpdateRecords { get; set; }
}

public class UpdateEquipmentEnergyProfileCommandHandler : IRequestHandler<UpdateEquipmentEnergyProfileCommand>
{
    private readonly IEquipmentEnergyService _equipmentEnergyService;
    private readonly ICryptoService _cryptoService;
    private readonly IMessageService _messageService;
    private readonly ILogger<UpdateEquipmentEnergyProfileCommandHandler> _logger;

    public UpdateEquipmentEnergyProfileCommandHandler(
        IEquipmentEnergyService equipmentEnergyService,
        ICryptoService cryptoService,
        IMessageService messageService,
        ILogger<UpdateEquipmentEnergyProfileCommandHandler> logger)

    {
        _equipmentEnergyService = equipmentEnergyService ?? throw new ArgumentNullException(nameof(equipmentEnergyService));
        _cryptoService = cryptoService ?? throw new ArgumentNullException(nameof(cryptoService));
        _messageService = messageService ?? throw new ArgumentNullException(nameof(messageService));
        _logger = logger;
    }

    public async Task Handle(UpdateEquipmentEnergyProfileCommand request, CancellationToken cancellationToken)
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

            await _equipmentEnergyService.UpdateEquipmentEnergy(request.EquipmentId, date, request.UpdateRecords);
            await _equipmentEnergyService.UpdateEquipmentDailyEnergy(request.EquipmentId, date, request.UpdateRecords);
            await _messageService.SendProgressAsync(100 * day / days, MessageScope.UpdateEquipmentEnergyTable);

            date = date.AddDays(1);
            day++;
        }
    }
}
