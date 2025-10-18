using MediatR;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using EnergyDashboard.Domain.Repository;

namespace EnergyDashboard.Application.LoadFeederTypes.Queries;

public class GetLoadFeederTypesQuery : IRequest<IEnumerable<LoadFeederTypeDto>>
{
}

public class GetLoadFeederTypesQueryHandler : IRequestHandler<GetLoadFeederTypesQuery, IEnumerable<LoadFeederTypeDto>>
{
    private readonly ILoadFeederTypeRepository _loadFeederTypeRepository;
    private readonly IMapper _mapper;

    public GetLoadFeederTypesQueryHandler(ILoadFeederTypeRepository loadFeederTypeRepository, IMapper mapper)
    {
        _loadFeederTypeRepository = loadFeederTypeRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<LoadFeederTypeDto>> Handle(GetLoadFeederTypesQuery request, CancellationToken cancellationToken)
    {
        var entitys = await _loadFeederTypeRepository.GetAllAsync(cancellationToken);
        return entitys.ProjectTo<LoadFeederTypeDto>(_mapper);
    }
}
