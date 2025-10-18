using MediatR;
using Application.Common.Exceptions;
using MeterService.Application.Interfaces;
using MeterService.Domain.Entities;

namespace MeterService.Application.Meters.Commands;

public sealed record UpdateMeterCommand : IRequest
{
    public int MeterId { get; set; }
    public string SerialNumber { get; set; }
    public string Name { get; set; }
    public bool Active { get; set; } = false;
    public bool Dismount { get; set; } = false;

    public string StationName { get; set; }
    public int? StationId { get; set; }

    public DateTimeOffset StartOperationDate { get; set; }
    public DateTimeOffset EndOperationDate { get; set; }
}

public sealed class UpdateMeterCommandHandler : IRequestHandler<UpdateMeterCommand>
{
    private readonly IApplicationDbContext _context;

    public UpdateMeterCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(UpdateMeterCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Meters
            .FindAsync(new object[] { request.MeterId }, cancellationToken);

        if (entity == null)
        {
            throw new NotFoundException(nameof(MeterEntity), request.MeterId);
        }

        entity.SerialNumber = request.SerialNumber;
        entity.Name = request.Name;
        entity.StationId = request.StationId;
        entity.StationName = request.StationName;
        entity.Active = request.Active;
        entity.Dismount = request.Dismount;
        entity.StartOperationDate = request.StartOperationDate;
        entity.EndOperationDate = request.EndOperationDate;

        await _context.SaveChangesAsync(cancellationToken);
    }
}
