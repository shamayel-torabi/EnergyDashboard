namespace EnergyDashboard.Application.Interfaces;

public interface IDateTimeService
{
    DateTimeOffset Now { get; }
}
