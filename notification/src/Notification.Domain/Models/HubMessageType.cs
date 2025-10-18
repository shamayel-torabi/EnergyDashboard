
namespace Notification.Domain;

public enum HubMessageType : byte
{
    Success = 0,
    Error = 1,
    Warning = 2,
    Info = 3
}
