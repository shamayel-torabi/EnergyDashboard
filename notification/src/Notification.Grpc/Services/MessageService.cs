using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Notification.gRPC.Services;

public sealed class MessageService : IMessageService
{
    private readonly ILogger<MessageService> _logger;
    private readonly IServiceProvider _serviceProvider;

    public MessageService(IServiceProvider serviceProvider, ILogger<MessageService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<IEnumerable<TextMessage>> GetMessagesAsync()
    {
        var messages = new List<TextMessage>();

        try
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var client = scope.ServiceProvider.GetRequiredService<Notification.NotificationClient>();
                var reply = await client.GetMessagesAsync(new Empty());
                messages.AddRange(reply.Messages.ToList());
            }
        }
        catch (Exception)
        {
            _logger.LogWarning("Error GetMessagesAsync");
        }

        return messages;
    }

    public async Task<IEnumerable<TextMessage>> GetMessageByDateAsync(DateTimeOffset date)
    {
        var messages = new List<TextMessage>();

        try
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var client = scope.ServiceProvider.GetRequiredService<Notification.NotificationClient>();
                var reply = await client.GetMessageByDateAsync(new MessageRequest { Date = date.ToTimestamp() });
                messages.AddRange(reply.Messages.ToList());
            }
        }
        catch (Exception)
        {
            _logger.LogWarning("Error GetMessagesAsync");
        }

        return messages;
    }

    public async Task<Guid> DeleteMessageAsync(Guid messageId)
    {
        Guid res = Guid.Empty;
        try
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var client = scope.ServiceProvider.GetRequiredService<Notification.NotificationClient>();
                var reply = await client.DeleteMessageAsync(new DeleteMessageRequest { MessageId = messageId.ToString() });
                res = new Guid(reply.MessageId);
            }
        }
        catch (Exception)
        {
            _logger.LogWarning("Error DeleteMessageAsync");
        }
        return res;
    }

    public async Task DeleteMessageByDateAsync(DateTimeOffset date)
    {
        try
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var client = scope.ServiceProvider.GetRequiredService<Notification.NotificationClient>();
                var reply = await client.DeleteMessageByDateAsync(new MessageRequest { Date = date.ToTimestamp() });
            }
        }
        catch (Exception)
        {
            _logger.LogWarning("Error DeleteMessageAsync");
        }
    }

    public async Task SendMessageAsync(string text, MessageScope messageScope, MessageType messageType, bool save = true)
    {
        var message = new TextMessage()
        {
            Id = Guid.NewGuid().ToString(),
            Text = text,
            MessageScope = messageScope,
            MessageType = messageType,
            Date = Timestamp.FromDateTimeOffset(DateTimeOffset.Now),
            Save = save
        };

        try
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var client = scope.ServiceProvider.GetRequiredService<Notification.NotificationClient>();
                var reply = await client.SendTextAsync(message);
            }
        }
        catch (Exception)
        {
            _logger.LogWarning($"Error SendMessageAsync: {text}");
        }
    }

    public async Task SendProgressAsync(int progress, MessageScope messageScope)
    {
        var message = new ProgressMessage()
        {
            Progress = progress,
            MessageScope = messageScope
        };

        try
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var client = scope.ServiceProvider.GetRequiredService<Notification.NotificationClient>();
                var reply = await client.SendProgressAsync(message);
            }
        }
        catch (Exception)
        {
            _logger.LogWarning($"Error SendProgressAsync: {progress}");
        }
    }

    public async Task SendProgressTextAsync(string text, MessageScope messageScope)
    {
        var message = new ProgressTextMessage()
        {
            Text = text,
            MessageScope = messageScope
        };
        try
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var client = scope.ServiceProvider.GetRequiredService<Notification.NotificationClient>();
                var reply = await client.SendProgressTextAsync(message);
            }
        }
        catch (Exception)
        {
            _logger.LogWarning($"Error SendProgressTextAsync: {text}");
        }
    }

    public async Task SendRefreshMeterInstantAsync()
    {
        var message = new RefreshMeterInstantMessage()
        {
        };

        try
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var client = scope.ServiceProvider.GetRequiredService<Notification.NotificationClient>();
                var reply = await client.SendRefreshMeterInstantAsync(message);
            }
        }
        catch (Exception)
        {
            _logger.LogWarning("Error SendRefreshMeterInstantAsync");
        }
    }

    public async Task SendRefreshEnergyProfileAsync(DateTimeOffset startDate, DateTimeOffset endDate)
    {
        var message = new RefreshEnergyProfileMessage
        {
            StartDate = startDate.ToTimestamp(),
            EndDate = endDate.ToTimestamp()
        };

        try
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var client = scope.ServiceProvider.GetRequiredService<Notification.NotificationClient>();
                var reply = await client.SendRefreshEnergyProfileAsync(message);
            }
        }
        catch (Exception)
        {
            _logger.LogWarning("Error SendRefreshMeterInstantAsync");
        }
    }
}