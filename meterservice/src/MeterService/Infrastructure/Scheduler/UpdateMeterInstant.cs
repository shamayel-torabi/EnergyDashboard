using MeterService.Application.Interfaces;
using Infrastructure.Common.Services;
using JobSchedule;

namespace MeterService.Infrastructure.Scheduler;

public sealed class UpdateMeterInstant : Job
{
    private readonly IModamService _modamService;
    private readonly IDateTimeService _dateTimeService;

    public UpdateMeterInstant(IServiceProvider serviceProvider,  string schedule) 
        : base(serviceProvider, schedule)
    {
        _modamService = serviceProvider.GetRequiredService<IModamService>();
        _dateTimeService = serviceProvider.GetRequiredService<IDateTimeService>();
    }

    public override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var startDate = _dateTimeService.Now.AddMinutes(-5.0);
        var endDate = _dateTimeService.Now;
        await _modamService.UpdateMeterInstantAsync(startDate, endDate, null, stoppingToken);
    }
}
