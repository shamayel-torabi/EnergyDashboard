using AutoMapper;
using MediatR;
using Application.Common.Exceptions;
using EnergyDashboard.Domain.Entities;
using EnergyDashboard.Domain.Repository;

namespace EnergyDashboard.Application.DailyEnergys.Queries;

public class GetDailyEnergyQuery : IRequest<DailyEnergyDto>
{
    public Guid Id { get; set; }
}

public class GetDailyEnergyQueryHandler : IRequestHandler<GetDailyEnergyQuery, DailyEnergyDto>
{
    private readonly IDailyEnergyRepository _dailyEnergyRepository;
    private readonly IMapper _mapper;

    public GetDailyEnergyQueryHandler(IDailyEnergyRepository dailyEnergyRepository, IMapper mapper)
    {
        _dailyEnergyRepository = dailyEnergyRepository;
        _mapper = mapper;
    }

    public async Task<DailyEnergyDto> Handle(GetDailyEnergyQuery request, CancellationToken cancellationToken)
    {
        var entity = await _dailyEnergyRepository.GetByIdAsync(request.Id , cancellationToken);

        if (entity is null)
            throw new NotFoundException(nameof(DailyEnergy), request.Id);

        var ret = _mapper.Map<DailyEnergyDto>(entity);
        return ret;
    }
}
