#nullable disable

namespace Notification.Domain;

public sealed class HubTextMessage
{
    public Guid Id { get; set; }
    public string Text { get; set; }
    public DateTimeOffset Date { get; set; }
    public bool Save { get; set; }
    public HubMessageScope MessageScope { get; set; }
    public HubMessageType MessageType { get; set; }
}
