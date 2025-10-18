using Application.Common.Exceptions;
using EnergyDashboard.Domain.Diagram;
using EnergyDashboard.Domain.Entities;
using EnergyDashboard.Domain.Repository;
using MediatR;

namespace EnergyDashboard.Application.Areas.Queries;

public class GetAreaDiagramQuery : IRequest<DiagramModel>
{
    public Guid Id { get; set; }
}

public class GetAreaDiagramQueryHandler : IRequestHandler<GetAreaDiagramQuery, DiagramModel>
{
    private readonly IAreaRepository _areaRepository;

    public GetAreaDiagramQueryHandler(IAreaRepository areaRepository)
    {
        _areaRepository = areaRepository;
    }

    public async Task<DiagramModel> Handle(GetAreaDiagramQuery request, CancellationToken cancellationToken)
    {
        var area = await _areaRepository.GetByIdAsync( request.Id , cancellationToken);

        if (area is null)
            throw new NotFoundException(nameof(Area), request.Id);

        return area.Diagram;
    }
}
