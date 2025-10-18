using AutoMapper;
using AutoMapper.QueryableExtensions;
using EnergyDashboard.Domain.Repository;
using MediatR;

namespace EnergyDashboard.Application.PowerplantTypes.Queries;

public class GetPowerplantTypesQuery : IRequest<IEnumerable<PowerplantTypeDto>>
{
}

public class GetPowerplantTypesQueryHandler : IRequestHandler<GetPowerplantTypesQuery, IEnumerable<PowerplantTypeDto>>
{
    private readonly IPowerplantTypeRepository _powerplantTypeRepository;
    private readonly IMapper _mapper;

    public GetPowerplantTypesQueryHandler(IPowerplantTypeRepository powerplantTypeRepository, IMapper mapper)
    {
        _powerplantTypeRepository = powerplantTypeRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<PowerplantTypeDto>> Handle(GetPowerplantTypesQuery request, CancellationToken cancellationToken)
    {
        var entitys = await _powerplantTypeRepository.GetAllAsync(cancellationToken);
        return entitys.ProjectTo<PowerplantTypeDto>(_mapper);
    }
}
