using Application.Common.Exceptions;
using EnergyDashboard.Domain.Diagram;
using EnergyDashboard.Domain.Entities;
using EnergyDashboard.Domain.Repository;
using MediatR;

namespace EnergyDashboard.Application.Networks.Queries;

public class GetNetworkDiagramQuery : IRequest<DiagramModel>
{
    public Guid Id { get; set; }
}

public class GetNetworkDiagramQueryHandler : IRequestHandler<GetNetworkDiagramQuery, DiagramModel>
{
    private readonly INetworkRepository _networkRepository;

    public GetNetworkDiagramQueryHandler(INetworkRepository networkRepository)
    {
        _networkRepository = networkRepository;
    }

    public async Task<DiagramModel> Handle(GetNetworkDiagramQuery request, CancellationToken cancellationToken)
    {
        var network = await _networkRepository.GetByIdAsync(request.Id , cancellationToken);

        if (network is null)
            throw new NotFoundException(nameof(Network), request.Id);

        return network.Diagram;
    }
}
