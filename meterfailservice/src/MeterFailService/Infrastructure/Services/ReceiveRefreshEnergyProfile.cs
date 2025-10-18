using MeterFailService.Application.Interfaces;
using MeterFailService.Domain.Entities;
using MeterService.gRPC.Services;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.EntityFrameworkCore;
using Notification.Domain;

namespace MeterFailService.Infrastructure.Services;

public sealed class ReceiveRefreshEnergyProfile : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<ReceiveRefreshEnergyProfile> _logger;
    private readonly HubConnection connection;

    public ReceiveRefreshEnergyProfile(IServiceProvider serviceProvider, string hubUrl)
    {
        _serviceProvider = serviceProvider;

        var loggerFactory = _serviceProvider.GetService<ILoggerFactory>();
        _logger = loggerFactory.CreateLogger<ReceiveRefreshEnergyProfile>();

        connection = new HubConnectionBuilder()
            .WithUrl(hubUrl)
            .WithAutomaticReconnect()
            .Build();

        connection.Reconnecting += (error) =>
        {
            _logger.LogInformation("Hub Connection Lost");
            return Task.CompletedTask;
        };

        connection.Closed += async (error) =>
        {
            await Task.Delay(new Random().Next(0, 5) * 1000);
            await connection.StartAsync();
            _logger.LogInformation("Start HubConnection");
        };

        connection.On<HubRefreshEnergyProfileMessage>("RefreshEnergyProfile", HandleRefreshEnergyProfile);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        bool loop = true;
        while (loop)
        {
            try
            {
                await connection.StartAsync(stoppingToken);
                _logger.LogInformation("Connection Stablished");
                loop = false;
            }
            catch when (stoppingToken.IsCancellationRequested)
            {
                loop = false;
            }
            catch
            {
                _logger.LogInformation("Try HubConnection after 5 secound");
                await Task.Delay(5000, stoppingToken);
            }
        }
    }

    private async Task HandleRefreshEnergyProfile(HubRefreshEnergyProfileMessage message)
    {

        var date = message.StartDate.Date;

        while(date <= message.EndDate.Date)
        {
            await SaveToDatabase(date);
            date = date.AddDays(1);
        }
        _logger.LogInformation($"Update EnergyProfile: from: {message.StartDate.ToShortDateString()}, to: {message.EndDate.ToShortDateString()}");
    }

    private async Task SaveToDatabase(DateTimeOffset recordDate)
    {
        using var scope = _serviceProvider.CreateScope();
        IApplicationDbContext dbContext = scope.ServiceProvider.GetService<IApplicationDbContext>();
        IMeterClient meterReaingService = scope.ServiceProvider.GetService<IMeterClient>();
        ICryptoService cryptoService = scope.ServiceProvider.GetService<ICryptoService>();

        var metersEnergy = await meterReaingService.GetMetersEnergy(recordDate);
        var meters = await meterReaingService.GetMeters();

        var meterEnergyList = metersEnergy.GroupBy(g => g.MeterId)
            .Select(s => new MeterEnergyEntity
            {
                MeterId = s.Key,
                RecordDate = recordDate.Date,
                HourEnergys = s.Select(x => new HourEnergy(
                    x.RecordDate.ToDateTimeOffset().Hour,
                    x.EnergyActiveExport,
                    x.EnergyActiveImport,
                    x.EnergyReactiveExport,
                    x.EnergyReactiveImport
                )).OrderBy(o => o.Hour).ToList()
            }).ToList();

        foreach (var m in meterEnergyList)
        {
            var meter = meters.FirstOrDefault(w => w.MeterId == m.MeterId);

            if (meter is not null)
            {
                Guid guid = cryptoService.GetDeterministicGuid(meter.SerialNumber, m.RecordDate.Ticks);

                if (dbContext.MeterEnergys.Any(f => f.Id == guid))
                {
                    MeterEnergyEntity entity = await dbContext.MeterEnergys.FirstOrDefaultAsync(f => f.Id == guid);
                    entity.Update(m.HourEnergys);
                    dbContext.MeterEnergys.Update(entity);
                }
                else
                {
                    MeterEnergyEntity entity = MeterEnergyEntity.Create(guid, meter.SerialNumber, m.RecordDate, m.HourEnergys);
                    await dbContext.MeterEnergys.AddAsync(entity);
                }
            }
        }

        await dbContext.SaveChangesAsync();
    }
}
