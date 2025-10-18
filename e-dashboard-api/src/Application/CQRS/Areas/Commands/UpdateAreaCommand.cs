using EnergyDashboard.Domain.Diagram;
using EnergyDashboard.Domain.Repository;
using MediatR;

namespace EnergyDashboard.Application.Areas.Commands;

public class UpdateAreaCommand : IRequest<DiagramModel>
{
    public Guid Id { get; set; }
    public DiagramModel Diagram { get; set; }
}

public class UpdateAreaCommandHandler : IRequestHandler<UpdateAreaCommand, DiagramModel>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAreaRepository _areaRepository;

    public UpdateAreaCommandHandler(IUnitOfWork unitOfWork, IAreaRepository areaRepository)
    {
        _unitOfWork = unitOfWork;
        _areaRepository = areaRepository;
    }

    public async Task<DiagramModel> Handle(UpdateAreaCommand command, CancellationToken cancellationToken)
    {
        await _areaRepository.UpdateDiagram(command.Id, command.Diagram, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return command.Diagram;
    }
}
