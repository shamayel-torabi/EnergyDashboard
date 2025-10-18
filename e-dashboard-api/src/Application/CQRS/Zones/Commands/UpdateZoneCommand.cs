using EnergyDashboard.Domain.Diagram;
using EnergyDashboard.Domain.Repository;
using MediatR;

namespace EnergyDashboard.Application.Zones.Commands;

public class UpdateZoneCommand : IRequest<DiagramModel>
{
    public Guid Id { get; set; }
    public DiagramModel Diagram { get; set; }
}

public class UpdateZoneCommandHandler : IRequestHandler<UpdateZoneCommand, DiagramModel>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IZoneRepository _zoneRepository;

    public UpdateZoneCommandHandler(IUnitOfWork unitOfWork, IZoneRepository zoneRepository)
    {
        _unitOfWork = unitOfWork;
        _zoneRepository = zoneRepository;
    }

    public async Task<DiagramModel> Handle(UpdateZoneCommand command, CancellationToken cancellationToken)
    {
        await _zoneRepository.UpdateDiagram(command.Id, command.Diagram, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return command.Diagram;
    }
}
