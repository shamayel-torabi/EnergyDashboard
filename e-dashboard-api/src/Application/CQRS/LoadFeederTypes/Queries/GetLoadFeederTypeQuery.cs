using MediatR;
using AutoMapper;
using Application.Common.Exceptions;
using EnergyDashboard.Domain.Entities;
using EnergyDashboard.Domain.Repository;

namespace EnergyDashboard.Application.LoadFeederTypes.Queries;

public class GetLoadFeederTypeQuery : IRequest<LoadFeederTypeDto>
{
    public int Id { get; set; }
}

public class GetLoadFeederTypeQueryHandler : IRequestHandler<GetLoadFeederTypeQuery, LoadFeederTypeDto>
{
    private readonly ILoadFeederTypeRepository _loadFeederTypeRepository;
    private readonly IMapper _mapper;

    public GetLoadFeederTypeQueryHandler(ILoadFeederTypeRepository loadFeederTypeRepository, IMapper mapper)
    {
        _loadFeederTypeRepository = loadFeederTypeRepository;
        _mapper = mapper;
    }

    public async Task<LoadFeederTypeDto> Handle(GetLoadFeederTypeQuery request, CancellationToken cancellationToken)
    {
        var entity = await _loadFeederTypeRepository.GetByIdAsync( request.Id , cancellationToken);

        if (entity is null)
            throw new NotFoundException(nameof(LoadFeederType), request.Id);

        return _mapper.Map<LoadFeederTypeDto>(entity);
    }
}
