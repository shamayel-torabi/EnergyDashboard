

namespace Notification.gRPC.Services;

public interface IMessageService
{
    Task<IEnumerable<TextMessage>> GetMessagesAsync();
    Task<IEnumerable<TextMessage>> GetMessageByDateAsync(DateTimeOffset date);
    Task<Guid> DeleteMessageAsync(Guid messageId);
    Task DeleteMessageByDateAsync(DateTimeOffset date);

    Task SendMessageAsync(string text, MessageScope messageScope, MessageType messageType, bool save = true);
    Task SendProgressAsync(int progress, MessageScope messageScope);
    Task SendProgressTextAsync(string text, MessageScope messageScope);
    Task SendRefreshMeterInstantAsync();
    Task SendRefreshEnergyProfileAsync(DateTimeOffset startDate, DateTimeOffset endDate);
}