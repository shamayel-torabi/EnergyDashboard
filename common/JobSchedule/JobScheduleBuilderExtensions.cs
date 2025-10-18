using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System.Reflection;
using JobSchedule;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class JobScheduleBuilderExtensions
    {
        public static IServiceCollection AddJobScheduledService(this IServiceCollection services, Assembly assembly )
        {
            services.TryAddEnumerable(
                ServiceDescriptor.Singleton<IConfigureOptions<JobScheduleOptions>, ConfigureJobSchedule>(sp =>
                {
                    var jobScheduleOptions = sp.GetRequiredService<IConfiguration>().GetSection("JobSchedules");
                    return new ConfigureJobSchedule(jobScheduleOptions);
                }));

            services.AddSingleton<IJobScheduleHandler, JobScheduleHandler>(sp =>
            {
                var logger = sp.GetRequiredService<ILogger<JobScheduleHandler>>();
                var options = sp.GetRequiredService<IOptions<JobScheduleOptions>>();
                return new JobScheduleHandler(sp, logger, options, assembly);
            });

            services.AddHostedService<JobScheduleService>(sp => {
                var jobScheduleHandler = sp.GetRequiredService<IJobScheduleHandler>();
                return new JobScheduleService(jobScheduleHandler);          
            });
            return services;
        }
    }
}
