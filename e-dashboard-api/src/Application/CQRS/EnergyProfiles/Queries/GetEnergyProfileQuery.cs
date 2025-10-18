using MediatR;
using AutoMapper;
using Application.Common.Exceptions;
using EnergyDashboard.Domain.Entities;
using EnergyDashboard.Domain.Repository;

namespace EnergyDashboard.Application.EnergyProfiles.Queries;

public class GetEnergyProfileQuery : IRequest<EnergyProfileDto>
{
    public Guid Id { get; set; }
}

public class GetEnergyProfileQueryHandler : IRequestHandler<GetEnergyProfileQuery, EnergyProfileDto>
{
    private readonly IEnergyProfileRepository _energyProfileRepository;
    private readonly IMapper _mapper;

    public GetEnergyProfileQueryHandler(IEnergyProfileRepository energyProfileRepository, IMapper mapper)
    {
        _energyProfileRepository = energyProfileRepository;
        _mapper = mapper;
    }

    public async Task<EnergyProfileDto> Handle(GetEnergyProfileQuery request, CancellationToken cancellationToken)
    {
        var entity = await _energyProfileRepository.GetByIdAsync(request.Id , cancellationToken);

        if (entity is null)
            throw new NotFoundException(nameof(EnergyProfile), request.Id);


        return _mapper.Map<EnergyProfileDto>(entity);
    }
}
