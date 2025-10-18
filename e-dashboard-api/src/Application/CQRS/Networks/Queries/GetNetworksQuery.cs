using AutoMapper;
using EnergyDashboard.Domain.Repository;
using MediatR;

namespace EnergyDashboard.Application.Networks.Queries;

public class GetNetworksQuery : IRequest<IEnumerable<NetworkDto>>
{
}

public class GetNetworksQueryHandler : IRequestHandler<GetNetworksQuery, IEnumerable<NetworkDto>>
{
    private readonly INetworkRepository _networkRepository;
    private readonly IMapper _mapper;

    public GetNetworksQueryHandler(INetworkRepository networkRepository, IMapper mapper)
    {
        _networkRepository = networkRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<NetworkDto>> Handle(GetNetworksQuery request, CancellationToken cancellationToken)
    {
        var networks = await _networkRepository.GetAllNetwork(cancellationToken);
        List<NetworkDto> ret = new();

        foreach (var network in networks)
        {
            var n = _mapper.Map<NetworkDto>(network);
            ret.Add(n);
        }

        return ret;
    }
}
