using EnergyDashboard.Application.Interfaces;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Notification.Domain;

namespace EnergyDashboard.Infrastructure.Services;

public class ReceiveRefreshEnergyProfile : BackgroundService
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

        while (date <= message.EndDate.Date)
        {
            await SaveToDatabase(date);
            date = date.AddDays(1);
        }
    }

    private async Task SaveToDatabase(DateTimeOffset recordDate)
    {
        using var scope = _serviceProvider.CreateScope();
        IEquipmentEnergyService equipmentEnergyService = scope.ServiceProvider.GetService<IEquipmentEnergyService>();
        
        await equipmentEnergyService.UpdateEquipmentsEnergy(recordDate, false);
        await equipmentEnergyService.UpdateEquipmentsDailyEnergy(recordDate, false);
    }
}
