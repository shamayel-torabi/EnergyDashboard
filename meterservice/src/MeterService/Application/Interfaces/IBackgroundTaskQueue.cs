using MeterService.Application.Models;

namespace MeterService.Application.Interfaces;

public interface IBackgroundTaskQueue
{
    ValueTask QueueAsync(QueueWorkItem workItem);
    ValueTask<QueueWorkItem> DequeueAsync(CancellationToken cancellationToken);
}
