namespace Notification.Domain;

public sealed class HubProgressMessage
{
    public int Progress { get; set; }
    public HubMessageScope MessageScope { get; set; }
}
