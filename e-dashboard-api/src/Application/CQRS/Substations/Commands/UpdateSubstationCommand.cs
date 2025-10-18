using MediatR;
using EnergyDashboard.Domain.Diagram;
using EnergyDashboard.Domain.Repository;

namespace EnergyDashboard.Application.Substations.Commands;

public class UpdateSubstationCommand : IRequest
{
    public Guid Id { get; set; }
    public DiagramModel Diagram { get; set; }
}

public class UpdateSubstationsCommandHandler : IRequestHandler<UpdateSubstationCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ISubstationRepository _substationRepository;

    public UpdateSubstationsCommandHandler(IUnitOfWork unitOfWork, ISubstationRepository substationRepository)
    {
        _unitOfWork = unitOfWork;
        _substationRepository = substationRepository;
    }

    public async Task Handle(UpdateSubstationCommand request, CancellationToken cancellationToken)
    {
        await _substationRepository.UpdateDiagram(request.Id, request.Diagram, cancellationToken);
        await _unitOfWork.SaveChangesAsync();
    }
}
