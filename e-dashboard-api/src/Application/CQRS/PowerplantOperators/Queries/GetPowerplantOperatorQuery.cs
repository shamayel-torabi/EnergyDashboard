using MediatR;
using AutoMapper;
using Application.Common.Exceptions;
using EnergyDashboard.Domain.Entities;
using EnergyDashboard.Domain.Repository;

namespace EnergyDashboard.Application.PowerplantOperators.Queries;

public class GetPowerplantOperatorQuery : IRequest<PowerplantOperatorDto>
{
    public int Id { get; set; }
}

public class GetPowerplantOperatorQueryHandler : IRequestHandler<GetPowerplantOperatorQuery, PowerplantOperatorDto>
{
    private readonly IPowerplantOperatorRepository _powerplantOperatorRepository;
    private readonly IMapper _mapper;

    public GetPowerplantOperatorQueryHandler(IPowerplantOperatorRepository powerplantOperatorRepository, IMapper mapper)
    {
        _powerplantOperatorRepository = powerplantOperatorRepository;
        _mapper = mapper;
    }

    public async Task<PowerplantOperatorDto> Handle(GetPowerplantOperatorQuery request, CancellationToken cancellationToken)
    {
        var entity = await _powerplantOperatorRepository.GetByIdAsync( request.Id , cancellationToken);

        if (entity is null)
            throw new NotFoundException(nameof(LoadFeederType), request.Id);

        return _mapper.Map<PowerplantOperatorDto>(entity);
    }
}
