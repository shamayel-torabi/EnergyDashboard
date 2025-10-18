using EnergyDashboard.Domain.Entities;
using EnergyDashboard.Domain.Repository;
using MediatR;

namespace EnergyDashboard.Application.Networks.Queries;

public class ExportNetworkQuery : IRequest<Network>
{
    public Guid Id { get; set; }
}

public class ExportNetworkQueryHandler : IRequestHandler<ExportNetworkQuery, Network>
{
    private readonly INetworkRepository _networkRepository;

    public ExportNetworkQueryHandler(INetworkRepository networkRepository)
    {
        _networkRepository = networkRepository;
    }

    public async Task<Network> Handle(ExportNetworkQuery request, CancellationToken cancellationToken)
    {
        return await _networkRepository.Export(request.Id, cancellationToken);
    }
}

