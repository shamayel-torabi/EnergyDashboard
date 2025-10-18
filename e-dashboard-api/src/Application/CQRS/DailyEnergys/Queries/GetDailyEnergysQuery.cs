using AutoMapper;
using MediatR;
using AutoMapper.QueryableExtensions;
using EnergyDashboard.Domain.Repository;

namespace EnergyDashboard.Application.DailyEnergys.Queries;

public class GetDailyEnergysQuery : IRequest<IEnumerable<DailyEnergyDto>>
{
}

public class GetDailyEnergysQueryHandler : IRequestHandler<GetDailyEnergysQuery, IEnumerable<DailyEnergyDto>>
{
    private readonly IDailyEnergyRepository _dailyEnergyRepository;
    private readonly IMapper _mapper;

    public GetDailyEnergysQueryHandler(IDailyEnergyRepository dailyEnergyRepository, IMapper mapper)
    {
        _dailyEnergyRepository = dailyEnergyRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<DailyEnergyDto>> Handle(GetDailyEnergysQuery request, CancellationToken cancellationToken)
    {
        var dailyEnergys = await _dailyEnergyRepository.GetAllAsync(cancellationToken);
        return dailyEnergys.ProjectTo<DailyEnergyDto>(_mapper);
    }
}
