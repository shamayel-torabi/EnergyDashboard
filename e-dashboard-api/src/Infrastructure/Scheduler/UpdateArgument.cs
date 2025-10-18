
using JobSchedule;

namespace EnergyDashboard.Infrastructure.Scheduler;

public class UpdateArgument : IArgument
{
    public int Interval { get; set; }

}
