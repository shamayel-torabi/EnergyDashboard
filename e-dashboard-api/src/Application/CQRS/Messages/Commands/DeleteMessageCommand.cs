using MediatR;
using Notification.gRPC.Services;

namespace EnergyDashboard.Application.Messages.Commands;

public class DeleteMessageCommand : IRequest<Guid>
{
    public Guid MessaageId { get; set; }
}

public class DeleteMessageCommandHandler : IRequestHandler<DeleteMessageCommand, Guid>
{
    private readonly IMessageService _messageService;
    public DeleteMessageCommandHandler(IMessageService messageService)
    {
        _messageService = messageService;
    }

    public async Task<Guid> Handle(DeleteMessageCommand request, CancellationToken cancellationToken)
    {
        return await _messageService.DeleteMessageAsync(request.MessaageId);
    }
}
