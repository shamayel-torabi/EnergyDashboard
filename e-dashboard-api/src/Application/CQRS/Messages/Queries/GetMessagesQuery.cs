using MediatR;
using Notification.gRPC.Services;

namespace EnergyDashboard.Application.Messages.Queries;

public class GetMessagesQuery : IRequest<IEnumerable<MessageDto>>
{
}

public class GetMessagesQueryHandler : IRequestHandler<GetMessagesQuery, IEnumerable<MessageDto>>
{
    private readonly IMessageService _messageService;
    public GetMessagesQueryHandler(IMessageService messageService)
    {
        _messageService = messageService;
    }


    public async Task<IEnumerable<MessageDto>> Handle(GetMessagesQuery request, CancellationToken cancellationToken)
    {
        var mm = await _messageService.GetMessagesAsync();
        var messages = new List<MessageDto>();

        foreach (var m in mm)
        {
            var message = new MessageDto
            {
                Id = new Guid(m.Id),
                Text = m.Text,
                Date = m.Date.ToDateTimeOffset(),
                MessageType = (HubMessageType) m.MessageType,
                MessageScope = (HubMessageScope) m.MessageScope,
            };

            messages.Add(message);
        }

        return messages;
    }
}
