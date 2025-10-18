
namespace JobSchedule
{
    public interface IJob
    {
        //Task ExecuteInScope(IServiceProvider serviceProvider, CancellationToken stoppingToken);

        Task ExecuteInScope(CancellationToken stoppingToken);
    }

    public interface  IJob<out TArgument> : IJob
    {
        TArgument Argument { get; }
    }
}
