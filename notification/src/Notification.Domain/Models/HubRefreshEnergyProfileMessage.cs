namespace Notification.Domain;

public sealed class HubRefreshEnergyProfileMessage
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}
