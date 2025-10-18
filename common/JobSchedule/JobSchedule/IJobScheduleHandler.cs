
namespace JobSchedule
{
    public interface IJobScheduleHandler
    {
        Task ExecuteAsync(CancellationToken stoppingToken);
    }
}
