using MessageBus;
using MeterFailService.Application.Interfaces;
using MeterFailService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MeterFailService.Application.MeterEnergy.MessageHandler;

public sealed class MeterEnergyMessageHandler : IMessageHandler<MeterEnergyMessage>
{
    private readonly ILogger<MeterEnergyMessageHandler> _logger;
    private readonly IApplicationDbContext _dbContext;
    private readonly ICryptoService _cryptoService;

    public MeterEnergyMessageHandler(
    ILogger<MeterEnergyMessageHandler> logger,
    IApplicationDbContext dbContext,
    ICryptoService cryptoService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _dbContext= dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        _cryptoService = cryptoService ?? throw new ArgumentNullException(nameof(cryptoService));
    }

    public async Task Handle(MeterEnergyMessage notification, CancellationToken cancellationToken)
    {
        //if (notification.Anomaly)
        //    return;

        try
        {
            Guid guid = _cryptoService.GetDeterministicGuid(notification.SerialNumber, notification.RecordDate.Ticks);

            if (_dbContext.MeterEnergys.Any(f => f.Id == guid))
            {
                MeterEnergyEntity entity = await _dbContext.MeterEnergys.FirstOrDefaultAsync(f => f.Id == guid);
                entity.Update(notification.HourEnergys);
                _dbContext.MeterEnergys.Update(entity);
            }
            else
            {
                MeterEnergyEntity entity = MeterEnergyEntity.Create(guid, notification.SerialNumber, notification.RecordDate, notification.HourEnergys);
                await _dbContext.MeterEnergys.AddAsync(entity, cancellationToken);
            }

            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError("Error", ex);
        }


        _logger.LogInformation($"Add Meter {notification.SerialNumber} at {notification.RecordDate.ToString("yyyy-MM-dd")}");
    }
}
