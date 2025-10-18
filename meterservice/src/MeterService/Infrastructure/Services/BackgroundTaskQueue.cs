using System.Threading.Channels;
using MeterService.Application.Interfaces;
using MeterService.Application.Models;

namespace MeterService.Infrastructure.Services;

public sealed class BackgroundTaskQueue : IBackgroundTaskQueue
{
    private readonly Channel<QueueWorkItem> _queue;

    public BackgroundTaskQueue(int capacity)
    {
        // Capacity should be set based on the expected application load and
        // number of concurrent threads accessing the queue.            
        // BoundedChannelFullMode.Wait will cause calls to WriteAsync() to return a task,
        // which completes only when space became available. This leads to backpressure,
        // in case too many publishers/calls start accumulating.
        var options = new BoundedChannelOptions(capacity)
        {
            FullMode = BoundedChannelFullMode.Wait
        };
        _queue = Channel.CreateBounded<QueueWorkItem>(options);
    }

    public async ValueTask QueueAsync(QueueWorkItem workItem)
    {
        if (workItem == null)
        {
            throw new ArgumentNullException(nameof(workItem));
        }

        await _queue.Writer.WriteAsync(workItem);
    }

    public async ValueTask<QueueWorkItem> DequeueAsync(CancellationToken cancellationToken)
    {
        QueueWorkItem workItem = null;
        try
        {
            workItem = await _queue.Reader.ReadAsync(cancellationToken);
        }
        catch (OperationCanceledException)
        {
        }
        return workItem;
    }
}
