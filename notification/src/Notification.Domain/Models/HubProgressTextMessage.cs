#nullable disable

namespace Notification.Domain;

public sealed class HubProgressTextMessage
{
    public string Text { get; set; }
    public HubMessageScope MessageScope { get; set; }
}
