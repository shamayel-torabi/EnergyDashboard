using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using NCrontab;
using static NCrontab.CrontabSchedule;

namespace JobSchedule
{
    public abstract class Job : IJob
    {
        private readonly CrontabSchedule _schedule;
        private readonly ILogger<Job> _logger;
        private DateTime _nextRun;

        public Job(IServiceProvider serviceProvider, string schedule)
        {
            ILoggerFactory logger = serviceProvider.GetService<ILoggerFactory>();
            _logger = logger.CreateLogger<Job>();

            try
            {
                _schedule = Parse(schedule, new ParseOptions { IncludingSeconds = false });
                var now = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Local);
                _nextRun = _schedule.GetNextOccurrence(now);
            }
            catch(Exception)
            {
                _logger.LogError($"Error Parse Schedule string {schedule}");
                _nextRun = DateTime.Now.AddHours(-1);
            }
        }

        public async Task ExecuteInScope(CancellationToken stoppingToken)
        {
            var now = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Local);
            if (now > _nextRun)
            {
                Task task = ExecuteAsync(stoppingToken);

                try
                {
                    await task;
                    var nextRunTime = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Local);
                    _nextRun = _schedule.GetNextOccurrence(nextRunTime);
                }
                catch
                {
                    var ex = task.Exception;
                    _logger.LogError(ex, ex.Message);
                }
            }
        }
        public abstract Task ExecuteAsync(CancellationToken stoppingToken);
    }

    public abstract class Job<TArgument> :Job , IJob<TArgument> where TArgument : IArgument , new()
    {
        public Job(IServiceProvider serviceProvider, string schedule, Dictionary<string, object> argument)
            : base(serviceProvider, schedule)
        {
            Argument = DictionaryToObject(argument);
        }
        public TArgument Argument { get; }

        private TArgument DictionaryToObject(Dictionary<string, object> dict)
        {
            TArgument t = new TArgument();
            PropertyInfo[] properties = t.GetType().GetProperties();

            foreach (PropertyInfo property in properties)
            {
                if (!dict.Any(x => x.Key.Equals(property.Name,
                    StringComparison.InvariantCultureIgnoreCase)))
                    continue;
                KeyValuePair<string, object> item = dict.First(x => x.Key.Equals(property.Name,
                    StringComparison.InvariantCultureIgnoreCase));
                Type tPropertyType = t.GetType().GetProperty(property.Name).PropertyType;
                Type newT = Nullable.GetUnderlyingType(tPropertyType) ?? tPropertyType;
                object newA = Convert.ChangeType(item.Value, newT);
                t.GetType().GetProperty(property.Name).SetValue(t, newA, null);
            }
            return t;
        }
    }
}
