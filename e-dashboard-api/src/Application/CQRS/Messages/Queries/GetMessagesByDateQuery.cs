using MediatR;
using Notification.gRPC.Services;

namespace EnergyDashboard.Application.Messages.Queries;

public class GetMessagesByDateQuery : IRequest<IEnumerable<MessageDto>>
{
    public DateTimeOffset Date { get; set; }
}

public class GetMessagesByDateQueryHandler : IRequestHandler<GetMessagesByDateQuery, IEnumerable<MessageDto>>
{
    private readonly IMessageService _messageService;
    public GetMessagesByDateQueryHandler(IMessageService messageService)
    {
        _messageService = messageService;
    }


    public async Task<IEnumerable<MessageDto>> Handle(GetMessagesByDateQuery request, CancellationToken cancellationToken)
    {
        var mm = await _messageService.GetMessageByDateAsync(request.Date);
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
