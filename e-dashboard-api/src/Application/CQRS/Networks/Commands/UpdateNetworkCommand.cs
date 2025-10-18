using EnergyDashboard.Domain.Diagram;
using EnergyDashboard.Domain.Repository;
using MediatR;

namespace EnergyDashboard.Application.Networks.Commands;

public class UpdateNetworkCommand : IRequest<DiagramModel>
{
    public Guid Id { get; set; }
    public DiagramModel Diagram { get; set; }
}

public class UpdateNetworkCommandHandler : IRequestHandler<UpdateNetworkCommand, DiagramModel>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly INetworkRepository _networkRepository;

    public UpdateNetworkCommandHandler(IUnitOfWork unitOfWork, INetworkRepository networkRepository)
    {
        _unitOfWork = unitOfWork;
        _networkRepository = networkRepository;
    }

    public async Task<DiagramModel> Handle(UpdateNetworkCommand command, CancellationToken cancellationToken)
    {
        await _networkRepository.UpdateDiagram(command.Id, command.Diagram, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return command.Diagram;
    }
}
