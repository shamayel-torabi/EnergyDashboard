using Application.Common.Exceptions;
using EnergyDashboard.Domain.Entities;
using EnergyDashboard.Domain.Repository;
using AutoMapper;
using MediatR;

namespace EnergyDashboard.Application.PowerplantTypes.Queries;

public class GetPowerplantTypeQuery : IRequest<PowerplantTypeDto>
{
    public int Id { get; set; }
}

public class GetPowerplantTypeQueryHandler : IRequestHandler<GetPowerplantTypeQuery, PowerplantTypeDto>
{
    private readonly IPowerplantTypeRepository _powerplantTypeRepository;
    private readonly IMapper _mapper;

    public GetPowerplantTypeQueryHandler(IPowerplantTypeRepository powerplantTypeRepository, IMapper mapper)
    {
        _powerplantTypeRepository = powerplantTypeRepository;
        _mapper = mapper;
    }

    public async Task<PowerplantTypeDto> Handle(GetPowerplantTypeQuery request, CancellationToken cancellationToken)
    {
        var entity = await _powerplantTypeRepository.GetByIdAsync(request.Id, cancellationToken);

        if (entity is null)
            throw new NotFoundException(nameof(PowerplantType), request.Id);

        return _mapper.Map<PowerplantTypeDto>(entity);
    }
}
