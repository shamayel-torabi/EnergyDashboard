using MediatR;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using EnergyDashboard.Domain.Repository;

namespace EnergyDashboard.Application.PowerplantOperators.Queries;

public class GetPowerplantOperatorsQuery : IRequest<IEnumerable<PowerplantOperatorDto>>
{
}

public class GetPowerplantOperatorsQueryHandler : IRequestHandler<GetPowerplantOperatorsQuery, IEnumerable<PowerplantOperatorDto>>
{
    private readonly IPowerplantOperatorRepository _powerplantOperatorRepository;
    private readonly IMapper _mapper;

    public GetPowerplantOperatorsQueryHandler(IPowerplantOperatorRepository powerplantOperatorRepository, IMapper mapper)
    {
        _powerplantOperatorRepository = powerplantOperatorRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<PowerplantOperatorDto>> Handle(GetPowerplantOperatorsQuery request, CancellationToken cancellationToken)
    {
        var entitys = await _powerplantOperatorRepository.GetAllAsync(cancellationToken);
        return entitys.ProjectTo<PowerplantOperatorDto>(_mapper);
    }
}
