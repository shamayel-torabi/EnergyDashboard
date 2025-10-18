using Microsoft.Extensions.Options;
using Microsoft.Extensions.Configuration;

namespace JobSchedule
{
    internal class ConfigureJobSchedule : IConfigureOptions<JobScheduleOptions>
    {
        private readonly IConfiguration _configuration;

        public ConfigureJobSchedule(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void Configure(JobScheduleOptions options)
        {
            foreach (var item in GetJobItem())
            {
                options.JobDefinitions.Add(item);
            }
        }

        internal IEnumerable<JobDefinition> GetJobItem()
        {
            var data = _configuration.Get<Dictionary<string, JobItemCollection>>();
            if (data != null)
            {
                foreach(var kvp in data)
                {
                    var name = kvp.Key;
                    var items = kvp.Value.JobItems;

                    foreach (var item in items)
                        yield return new JobDefinition { Name = name, Schedule = item.Schedule, Argument = item.Argument };
                }
            }
        }
    }
}
