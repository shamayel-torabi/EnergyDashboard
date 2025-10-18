using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Globalization;
using EnergyDashboard.Application.Interfaces;
using JobSchedule;

namespace EnergyDashboard.Infrastructure.Scheduler;

public class UpdateFailedMetersEnergy : Job
{
    private readonly ILogger<UpdateFailedMetersEnergy> _logger;
    private readonly IEquipmentEnergyService _equipmentEnergyService;

    public UpdateFailedMetersEnergy(IServiceProvider serviceProvider, string schedule) 
        : base(serviceProvider, schedule)
    {
        _logger = serviceProvider.GetService<ILoggerFactory>().CreateLogger<UpdateFailedMetersEnergy>();
        _equipmentEnergyService = serviceProvider.GetService<IEquipmentEnergyService>(); 
    }

    public override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var date = DateTime.Now;
        var culture = new CultureInfo("fa-IR");

        _logger.LogInformation($"UpdateFailedMetersEnergy start executing at - {date.ToString("yyyy/MM/dd HH:mm:ss", culture)}");
        await _equipmentEnergyService.ReadFailedMeters();
        _logger.LogInformation($"UpdateFailedMetersEnergy end executing at");
    }
}
