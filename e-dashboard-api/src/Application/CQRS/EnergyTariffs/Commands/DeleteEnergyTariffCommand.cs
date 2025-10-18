using MediatR;
using Application.Common.Exceptions;
using EnergyDashboard.Domain.Entities;
using EnergyDashboard.Domain.Repository;

namespace EnergyDashboard.Application.EnergyTariffs.Commands;

public class DeleteEnergyTariffCommand : IRequest<Guid>
{
    public Guid Id { get; set; }
}

public class DeleteEnergyTariffsCommandHandler : IRequestHandler<DeleteEnergyTariffCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEnergyTariffRepository _context;

    public DeleteEnergyTariffsCommandHandler(IUnitOfWork unitOfWork, IEnergyTariffRepository context)
    {
        _unitOfWork = unitOfWork;
        _context = context;
    }

    public async Task<Guid> Handle(DeleteEnergyTariffCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.GetByIdAsync(request.Id, cancellationToken);

        if (entity is null)
            throw new NotFoundException(nameof(EnergyTariff), request.Id);

        _context.Remove(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }
}
