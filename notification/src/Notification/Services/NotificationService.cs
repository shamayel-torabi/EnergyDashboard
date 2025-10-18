using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Notification.Data;
using Notification.Domain;
using Notification.gRPC;
using Notification.Hub;

namespace Notification.Services;

public sealed class NotificationService : gRPC.Notification.NotificationBase
{
    private readonly MessageDbContext _context;
    private readonly IHubContext<MessageHub, IMessageHub> _messageHubContext;
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(
        IHubContext<MessageHub, IMessageHub> messageHubContext,
        MessageDbContext context,
        ILogger<NotificationService> logger)
    {
        _messageHubContext = messageHubContext;
        _context = context;
        _logger = logger;
    }

    public override async Task<MessageResponse> GetMessages(Empty request, ServerCallContext context)
    {
        var messages = await _context.Messages.OrderBy(o => o.Date).ToListAsync();
        var messageResponse = new MessageResponse();

        foreach (var message in messages)
        {
            var m = new TextMessage();
            m.Id = message.Id.ToString();
            m.Text = message.Text;
            m.Date = message.Date.ToTimestamp();
            m.MessageType = (MessageType) message.MessageType;
            m.MessageScope = (MessageScope) (message.MessageScope.HasValue ? message.MessageScope.Value : HubMessageScope.General);
            messageResponse.Messages.Add(m);
        }

        return messageResponse;
    }

    public override async Task<MessageResponse> GetMessageByDate(MessageRequest request, ServerCallContext context)
    {
        var date = request.Date.ToDateTimeOffset();
        var messages = await _context.Messages.Where(w => w.Date.Date == date.Date).ToListAsync();

        var messageResponse = new MessageResponse();

        foreach (var message in messages)
        {
            var m = new TextMessage();
            m.Id = message.Id.ToString();
            m.Text = message.Text;
            m.Date = message.Date.ToTimestamp();
            m.MessageType = (MessageType)message.MessageType;
            m.MessageScope = (MessageScope)(message.MessageScope.HasValue ? message.MessageScope.Value : HubMessageScope.General);
            messageResponse.Messages.Add(m);
        }

        return messageResponse;
    }

    public override async Task<DeleteMessageRequest> DeleteMessage(DeleteMessageRequest request, ServerCallContext context)
    {
        var id = new Guid(request.MessageId);

        var message = await _context.Messages.SingleOrDefaultAsync(e => e.Id == id);

        if(message != null)
        {
            _context.Messages.Remove(message);
            await _context.SaveChangesAsync();
        }

        return request;
    }

    public override async Task<Empty> DeleteMessageByDate(MessageRequest request, ServerCallContext context)
    {
        var date = request.Date.ToDateTimeOffset();
        var s = new DateTime(date.Year, date.Month, date.Day, 0, 0, 0, 0);

        var e = s.AddDays(1);

        var messages = await _context.Messages.Where(w => w.Date >= s && w.Date < e)
            .AsNoTracking()
            .ToListAsync();

        if (messages != null)
        {
            _context.Messages.RemoveRange(messages);
            await _context.SaveChangesAsync();
        }

        return new Empty();
    }

    public override async Task<ReplyMessage> SendText(TextMessage request, ServerCallContext context)
    {
        var message = new HubTextMessage
        {
            Id = new Guid(request.Id),
            Text = request.Text,
            Date = request.Date.ToDateTimeOffset(),
            MessageScope = (HubMessageScope)request.MessageScope,
            MessageType = (HubMessageType)request.MessageType
        };

        if (request.Save)
            await SaveMessage(message);

        await _messageHubContext.Clients.All.MessageAdded(message);

        return new ReplyMessage { Succeed = true};
    }

    public override async Task<ReplyMessage> SendProgress(ProgressMessage request, ServerCallContext context)
    {
        _logger.LogInformation($"ProgressMessage:{request.Progress} , {request.MessageScope}");
        
        var message = new HubProgressMessage
        {
            Progress = request.Progress,
            MessageScope = (HubMessageScope)request.MessageScope,
        };
        await _messageHubContext.Clients.All.Progress(message);
        return new ReplyMessage { Succeed = true };
    }

    public override async Task<ReplyMessage> SendProgressText(ProgressTextMessage request, ServerCallContext context)
    {
        var message = new HubProgressTextMessage
        {
            Text = request.Text,
            MessageScope = (HubMessageScope)request.MessageScope,
        };

        await _messageHubContext.Clients.All.ProgressText(message);
        return new ReplyMessage { Succeed = true };
    }

    public override async Task<ReplyMessage> SendRefreshMeterInstant(RefreshMeterInstantMessage request, ServerCallContext context)
    {
        await _messageHubContext.Clients.All.RefreshMeterInstant();
        return new ReplyMessage { Succeed = true };
    }

    public override async Task<ReplyMessage> SendRefreshEnergyProfile(RefreshEnergyProfileMessage request, ServerCallContext context)
    {
        var message = new HubRefreshEnergyProfileMessage
        {
            StartDate = request.StartDate.ToDateTime(),
            EndDate= request.EndDate.ToDateTime(),
        };

        await _messageHubContext.Clients.All.RefreshEnergyProfile(message);
        return new ReplyMessage { Succeed = true };
    }

    private async Task SaveMessage(HubTextMessage message)
    {
        var msg = new MessageEntity
        {
            Id = message.Id,
            Text = message.Text,
            Date = message.Date,
            MessageType = message.MessageType,
            MessageScope =  message.MessageScope,
        };

        await _context.Messages.AddAsync(msg);
        await _context.SaveChangesAsync();
    }
}
