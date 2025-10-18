using MediatR;
using EnergyDashboard.Domain.Entities;
using Application.Common.Exceptions;
using EnergyDashboard.Domain.Repository;

namespace EnergyDashboard.Application.EnergyTariffs.Commands;

public class UpdateEnergyTariffCommand : IRequest
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public IEnumerable<EnergyTariffRate> EnergyTariffRates { get; set; }
}

public class UpdateEnergyTariffCommandHandler : IRequestHandler<UpdateEnergyTariffCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEnergyTariffRepository _context;

    public UpdateEnergyTariffCommandHandler(IUnitOfWork unitOfWork, IEnergyTariffRepository context)
    {
        _unitOfWork = unitOfWork;
        _context = context;
    }

    public async Task Handle(UpdateEnergyTariffCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.GetByIdAsync(request.Id, cancellationToken);

        if (entity is null)
            throw new NotFoundException(nameof(EnergyTariff), request.Id);

        entity.Update(
            request.Name,
            request.StartDate,
            request.EndDate,
            request.EnergyTariffRates);

        _context.Update(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
