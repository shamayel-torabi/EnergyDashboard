using MediatR;
using MeterService.Application.Interfaces;
using MeterService.Application.Models;
using Microsoft.Extensions.Options;

namespace MeterService.Application.MeterEnergy.Commands;

public sealed record DownloadMeterEnergyCommand : IRequest
{
    public DateTimeOffset StartDate { get; set; }
    public DateTimeOffset EndDate { get; set; }
    public string SerialNumber { get; set; }
    public bool Update { get; set; } = false;
}

public sealed class DownloadMeterEnergyCommandHandler : IRequestHandler<DownloadMeterEnergyCommand>
{
    private readonly IBackgroundTaskQueue _backgroundTaskQueue;
    private readonly IOptions<IGMCOptions> _option;
    private readonly ILogger<DownloadMeterEnergyCommandHandler> _logger;

    public DownloadMeterEnergyCommandHandler(
        IBackgroundTaskQueue backgroundTaskQueue,
        IOptions<IGMCOptions> option,
        ILogger<DownloadMeterEnergyCommandHandler> logger)
    {
        _backgroundTaskQueue = backgroundTaskQueue;
        _option = option;
        _logger = logger;
    }

    public async Task Handle(DownloadMeterEnergyCommand request, CancellationToken cancellationToken)
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
                    Update = request.Update,
                    SerialNumber = request.SerialNumber,
                });

            _logger.LogInformation($"Add QueueWorkItem from {startDate.ToString("yyyy/MM/dd")} to {endDate.ToString("yyyy/MM/dd")} for Meter {request.SerialNumber}");

            startDate = endDate.AddDays(1);
        }
    }
}
