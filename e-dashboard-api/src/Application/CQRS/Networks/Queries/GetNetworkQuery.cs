using Application.Common.Exceptions;
using EnergyDashboard.Domain.Diagram;
using EnergyDashboard.Domain.Entities;
using EnergyDashboard.Domain.Repository;
using MediatR;

namespace EnergyDashboard.Application.Networks.Queries;

public class GetNetworkQuery : IRequest<DiagramModel>
{
    public Guid Id { get; set; }
}

public class GetNetworkQueryHandler : IRequestHandler<GetNetworkQuery, DiagramModel>
{
    private readonly INetworkRepository _networkRepository;

    public GetNetworkQueryHandler(INetworkRepository networkRepository)
    {
        _networkRepository = networkRepository;
    }

    public async Task<DiagramModel> Handle(GetNetworkQuery request, CancellationToken cancellationToken)
    {
        var network = await _networkRepository.GetByIdAsync(request.Id, cancellationToken);

        if (network is null)
            throw new NotFoundException(nameof(Network), request.Id);

        DiagramModel model;

        if (network.Diagram is null)
        {
            model = new DiagramModel(network.Id.ToString(),"","network");
            model.Properties = new DiagramModelProperties();
            model.Properties.Title = network.Name;
            model.Properties.Description = network.Name;

        }
        else
        {
            model = network.Diagram;
        }

        return model;
    }
}
