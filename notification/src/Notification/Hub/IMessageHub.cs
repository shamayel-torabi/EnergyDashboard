


using Notification.Domain;

namespace Notification.Hub;

public interface IMessageHub
{
    Task MessageAdded(HubTextMessage message);
    Task Progress(HubProgressMessage message);
    Task ProgressText(HubProgressTextMessage message);
    Task RefreshMeterInstant();
    Task RefreshEnergyProfile(HubRefreshEnergyProfileMessage message);
}
