using Microsoft.Extensions.Hosting;

namespace JobSchedule
{
    public class JobScheduleService : BackgroundService
    {
        private readonly IJobScheduleHandler _jobScheduleHandler;

        public JobScheduleService( IJobScheduleHandler jobScheduleHandler)
        {
            _jobScheduleHandler = jobScheduleHandler;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _jobScheduleHandler.ExecuteAsync(stoppingToken);
        }
    }
}
