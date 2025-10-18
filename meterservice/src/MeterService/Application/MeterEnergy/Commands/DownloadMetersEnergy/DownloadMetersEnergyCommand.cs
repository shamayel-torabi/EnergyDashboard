using MediatR;
using MeterService.Application.Interfaces;
using MeterService.Application.Models;
using Microsoft.Extensions.Options;

namespace MeterService.Application.MeterEnergy.Commands;

public sealed record DownloadMetersEnergyCommand : IRequest
{
    public DateTimeOffset StartDate { get; set; }
    public DateTimeOffset EndDate { get; set; }
    public bool Update { get; set; } = false;
}

public sealed class DownloadMetersEnergyCommandHandler : IRequestHandler<DownloadMetersEnergyCommand>
{
    private readonly IBackgroundTaskQueue _backgroundTaskQueue;
    private readonly IOptions<IGMCOptions> _option;
    private readonly ILogger<DownloadMetersEnergyCommandHandler> _logger;

    public DownloadMetersEnergyCommandHandler(
        IBackgroundTaskQueue backgroundTaskQueue,
        IOptions<IGMCOptions> option,
        ILogger<DownloadMetersEnergyCommandHandler> logger)
    {
        _backgroundTaskQueue = backgroundTaskQueue;
        _option = option;
        _logger = logger;
    }


    public async Task Handle(DownloadMetersEnergyCommand request, CancellationToken cancellationToken)
    {
        var requestDays = _option.Value.RequestDays;
        var startDate = request.StartDate.Date;

        while (startDate < request.EndDate.Date)
        {
            if (cancellationToken.IsCancellationRequested)
                break;

            var endDate = startDate.AddDays(requestDays);
            if (endDate > request.EndDate.Date)
                endDate = request.EndDate.Date;

            await _backgroundTaskQueue.QueueAsync(
                new QueueWorkItem
                {
                    StartDate = startDate,
                    EndDate = endDate,
                    Update = request.Update
                });

            _logger.LogInformation($"Add QueueWorkItem from {startDate.ToString("yyyy/MM/dd")} to {endDate.ToString("yyyy/MM/dd")}");

            startDate = endDate.AddDays(1);
        }
    }
}
