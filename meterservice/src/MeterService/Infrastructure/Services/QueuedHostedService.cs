using MeterService.Application.Interfaces;
using MeterService.Application.Models;

namespace MeterService.Infrastructure.Services;

public sealed class QueuedHostedService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IBackgroundTaskQueue _taskQueue;
    private readonly ILogger<QueuedHostedService> _logger;

    public QueuedHostedService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        _taskQueue = serviceProvider.GetRequiredService<IBackgroundTaskQueue>();
        _logger = serviceProvider.GetRequiredService<ILoggerFactory>().CreateLogger<QueuedHostedService>();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var workItem = await _taskQueue.DequeueAsync(stoppingToken);
                    if(workItem != null)
                        await DoBackgroundTask(workItem, scope.ServiceProvider, stoppingToken);
                }
                catch(TaskCanceledException){
                    _logger.LogWarning("TaskCanceledException accure");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred executing WorkItem.");
                }
            }
        }
    }

    public override async Task StopAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Queued Hosted Service is stopping.");
        await base.StopAsync(stoppingToken);
    }

    private async Task DoBackgroundTask(QueueWorkItem workItem, IServiceProvider serviceProvider, CancellationToken stoppingToken)
    {
        bool result = false;
        int retry = 3;
        ISepacService sepacService = serviceProvider.GetRequiredService<ISepacService>();

        _logger.LogInformation($"Do QueueWorkItem from {workItem.StartDate.ToString("yyyy/MM/dd")} to {workItem.EndDate.ToString("yyyy/MM/dd")}");

        do
        {
            if (retry == 0)
                break;

            DateTime s = DateTime.Now;
            if (workItem.SerialNumber == null)
                result = await sepacService.DownloadMetersEnergy(workItem.StartDate, workItem.EndDate, workItem.Update, stoppingToken);
            else
                result = await sepacService.DownloadMeterEnergy(workItem.StartDate, workItem.EndDate, workItem.SerialNumber, workItem.Update, stoppingToken);

            int e = 906 - (int)DateTime.Now.Subtract(s).TotalSeconds;

            if (e > 0)
            {
                _logger.LogInformation($"Wait {e} Secound");
                await Task.Delay(TimeSpan.FromSeconds(e), stoppingToken);
            }
            retry--;

        } while (!result);
    }
}
