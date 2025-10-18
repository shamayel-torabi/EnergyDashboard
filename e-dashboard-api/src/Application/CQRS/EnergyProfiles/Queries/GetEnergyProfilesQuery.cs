using MediatR;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using EnergyDashboard.Domain.Repository;

namespace EnergyDashboard.Application.EnergyProfiles.Queries;

public class GetEnergyProfilesQuery : IRequest<IEnumerable<EnergyProfileDto>>
{
}

public class GetEnergyProfilesQueryHandler : IRequestHandler<GetEnergyProfilesQuery, IEnumerable<EnergyProfileDto>>
{
    private readonly IEnergyProfileRepository _energyProfileRepository;
    private readonly IMapper _mapper;

    public GetEnergyProfilesQueryHandler(IEnergyProfileRepository energyProfileRepository, IMapper mapper)
    {
        _energyProfileRepository = energyProfileRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<EnergyProfileDto>> Handle(GetEnergyProfilesQuery request, CancellationToken cancellationToken)
    {
        var entitys = await _energyProfileRepository.GetAllAsync(cancellationToken);
        return entitys.ProjectTo<EnergyProfileDto>(_mapper);
    }
}
