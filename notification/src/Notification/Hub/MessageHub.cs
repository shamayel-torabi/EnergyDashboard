using Microsoft.AspNetCore.SignalR;
using Notification.Domain;

namespace Notification.Hub;

public sealed class MessageHub: Hub<IMessageHub>
{
    public async Task SendMessage(HubTextMessage message)
    {
        await Clients.All.MessageAdded(message);
    }

    public async Task SendProgress(HubProgressMessage message)
    {
        await Clients.All.Progress(message);
    }

    public async Task SendProgressText(HubProgressTextMessage message)
    {
        await Clients.All.ProgressText(message);
    }

    public async Task SendRefreshMeterInstant()
    {
        await Clients.All.RefreshMeterInstant();
    }

    public async Task SendRefreshEnergyProfile(HubRefreshEnergyProfileMessage message)
    {
        await Clients.All.RefreshEnergyProfile(message);
    }
}
