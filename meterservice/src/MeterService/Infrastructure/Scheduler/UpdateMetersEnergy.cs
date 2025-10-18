using Microsoft.Extensions.Options;
using MeterService.Application.Models;
using MeterService.Application.Interfaces;
using Infrastructure.Common.Services;
using JobSchedule;

namespace MeterService.Infrastructure.Scheduler;

public sealed class UpdateMetersEnergy: Job<UpdateArgument>
{
    private readonly IBackgroundTaskQueue _backgroundTaskQueue;
    private readonly ILogger<UpdateMetersEnergy> _logger;
    private readonly IOptions<IGMCOptions> _option;
    private readonly IDateTimeService _dateTimeService;

    public UpdateMetersEnergy(IServiceProvider serviceProvider, string schedule, Dictionary<string, object> argument) 
        : base(serviceProvider, schedule, argument)
    {
        _backgroundTaskQueue = serviceProvider.GetRequiredService<IBackgroundTaskQueue>();
        _logger = serviceProvider.GetRequiredService<ILoggerFactory>().CreateLogger<UpdateMetersEnergy>();
        _option = serviceProvider.GetRequiredService<IOptions<IGMCOptions>>();
        _dateTimeService = serviceProvider.GetRequiredService<IDateTimeService>();
    }

    public override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var requestDays = _option.Value.RequestDays;
        var end = _dateTimeService.Now.Date.AddDays(-1);
        var start = end.AddDays(-Argument.Interval);

        var startDate = start.Date;

        while (startDate < end.Date)
        {
            if (stoppingToken.IsCancellationRequested)
                break;

            var endDate = startDate.AddDays(requestDays);
            if (endDate > end.Date)
                endDate = end.Date;

            await _backgroundTaskQueue.QueueAsync(
                new QueueWorkItem
                {
                    StartDate = startDate,
                    EndDate = endDate,
                    Update = false
                });

            _logger.LogInformation($"Add QueueWorkItem from {startDate.ToString("yyyy/MM/dd")} to {endDate.ToString("yyyy/MM/dd")}");

            startDate = endDate.AddDays(1);
        }
    }
}
