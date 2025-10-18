
using Notification.Domain;

namespace Notification.Data;

public sealed class MessageEntity
{
    public Guid Id { get; set; }
    public string Text { get; set; } = String.Empty;
    public DateTimeOffset Date { get; set; }
    public HubMessageType MessageType { get; set; }
    public HubMessageScope? MessageScope { get; set; }
}
