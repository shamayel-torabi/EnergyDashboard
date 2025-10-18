using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NCrontab;
using System.Reflection;
using static NCrontab.CrontabSchedule;

namespace JobSchedule
{
    public class JobScheduleHandler : IJobScheduleHandler
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<JobScheduleHandler> _logger;
        private readonly JobScheduleOptions _scheduleJobs;
        private readonly Assembly _assembly;

        private readonly CrontabSchedule _schedule;
        private List<IJob> _scheduledJob = new List<IJob>();
        private DateTime _nextRun;

        public JobScheduleHandler(
            IServiceProvider serviceProvider,
            ILogger<JobScheduleHandler> logger,
            IOptions<JobScheduleOptions> scheduleJobOptions,
            Assembly assembly)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            _assembly = assembly;
            _scheduleJobs = scheduleJobOptions.Value;

            _schedule = Parse("* * * * *", new ParseOptions { IncludingSeconds = false });
            var nextRunTime = DateTime.Now;
            _nextRun = _schedule.GetNextOccurrence(nextRunTime);
        }

        public async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                await ExecuteInScope(scope.ServiceProvider, stoppingToken);
            }
        }

        private async Task ExecuteInScope(IServiceProvider serviceProvider, CancellationToken stoppingToken)
        {
            AddJobs(serviceProvider);

            do
            {
                var now = DateTime.Now;
                if (now > _nextRun)
                {
                    foreach (var job in _scheduledJob)
                    {
                        await job.ExecuteInScope(stoppingToken);
                    }

                    _nextRun = _schedule.GetNextOccurrence(DateTime.Now);
                }
                await Task.Delay(500, stoppingToken); //500 milisecound delay
            }
            while (!stoppingToken.IsCancellationRequested);
        }

        private void AddJobs(IServiceProvider serviceProvider)
        {
            var jobItmes = _scheduleJobs.JobDefinitions;
            var jobTypes = GetScheduledJob();

            foreach (var jobItem in jobItmes)
            {
                var jobSchedule = jobTypes.FirstOrDefault(t => t.Name == jobItem.Name);
                IJob jobInstance = null;

                if (jobSchedule != null)
                {
                    bool isGeneric = jobSchedule.GetInterfaces()
                        .Any(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IJob<>));

                    if (isGeneric)
                        jobInstance = Activator.CreateInstance(jobSchedule, serviceProvider, jobItem.Schedule, jobItem.Argument) as IJob;
                    else
                        jobInstance = Activator.CreateInstance(jobSchedule, serviceProvider, jobItem.Schedule) as IJob;

                    _scheduledJob.Add(jobInstance);
                    _logger.LogInformation($"Add {jobItem.Name} Job Scheduled At:{jobItem.Schedule}");
                }
            }
        }

        private IEnumerable<Type> GetScheduledJob()
        {
            Type[] types = _assembly.GetTypes();

            IEnumerable<Type> imp = types.Where(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(Job)));
            return imp;
        }
    }
}
