
using JobSchedule;

namespace MeterService.Infrastructure.Scheduler;

public record UpdateArgument :IArgument
{
    public int Interval { get; set; }
}
