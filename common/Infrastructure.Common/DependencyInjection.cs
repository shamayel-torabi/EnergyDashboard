
using Infrastructure.Common.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Common
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddDateTimeService(this IServiceCollection services)
        {
            services.AddScoped<IDateTimeService, DateTimeService>();
            return services;
        }
    }
}