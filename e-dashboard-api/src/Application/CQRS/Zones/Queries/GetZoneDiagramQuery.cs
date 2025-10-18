using Application.Common.Exceptions;
using EnergyDashboard.Domain.Diagram;
using EnergyDashboard.Domain.Entities;
using EnergyDashboard.Domain.Repository;
using MediatR;

namespace EnergyDashboard.Application.Zones.Queries;

public class GetZoneDiagramQuery : IRequest<DiagramModel>
{
    public Guid Id { get; set; }
}

public class GetZoneDiagramQueryHandler : IRequestHandler<GetZoneDiagramQuery, DiagramModel>
{
    private readonly IZoneRepository _zoneRepository;

    public GetZoneDiagramQueryHandler(IZoneRepository zoneRepository)
    {
        _zoneRepository = zoneRepository;
    }

    public async Task<DiagramModel> Handle(GetZoneDiagramQuery request, CancellationToken cancellationToken)
    {
        var zone = await _zoneRepository.GetByIdAsync(request.Id , cancellationToken);

        if (zone is null)
            throw new NotFoundException(nameof(Zone), request.Id);

        return zone.Diagram;
    }
}
