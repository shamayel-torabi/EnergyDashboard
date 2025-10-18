using MediatR;
using Notification.gRPC.Services;

namespace EnergyDashboard.Application.Messages.Commands;

public class DeleteMessageByDateCommand : IRequest
{
    public DateTimeOffset Date { get; set; }
}

public class DeleteMessageByDateCommandHandler : IRequestHandler<DeleteMessageByDateCommand>
{
    private readonly IMessageService _messageService;
    public DeleteMessageByDateCommandHandler(IMessageService messageService)
    {
        _messageService = messageService;
    }

    public async Task Handle(DeleteMessageByDateCommand request, CancellationToken cancellationToken)
    {
        await _messageService.DeleteMessageByDateAsync(request.Date);
    }
}
