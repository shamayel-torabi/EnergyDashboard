using MediatR;
using Application.Common.Exceptions;
using MeterService.Application.Interfaces;
using MeterService.Domain.Entities;

namespace MeterService.Application.Meters.Commands;

public sealed record DeleteMeterCommand : IRequest
{
    public int MeterId { get; set; }
}

public sealed class DeleteMeterCommandHandler : IRequestHandler<DeleteMeterCommand>
{
    private readonly IApplicationDbContext _context;

    public DeleteMeterCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(DeleteMeterCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Meters
            .FindAsync(new object[] { request.MeterId }, cancellationToken);

        if (entity == null)
        {
            throw new NotFoundException(nameof(MeterEntity), request.MeterId);
        }

        _context.Meters.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
