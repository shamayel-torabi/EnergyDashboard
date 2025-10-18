using System.Reflection;
using FluentValidation;
using MediatR;
using MeterFailService.Application.Common.Behaviours;

namespace MeterFailService.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        string redisConnection = configuration.GetValue<string>("RedisConnection");

        services.AddAutoMapper(Assembly.GetExecutingAssembly());
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        //services.AddRedisMessageBus(op =>
        //{
        //    op.ConnectionString = redisConnection;
        //    op.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
        //});

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(UnhandledExceptionBehaviour<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(PerformanceBehaviour<,>));
        return services;
    }
}
