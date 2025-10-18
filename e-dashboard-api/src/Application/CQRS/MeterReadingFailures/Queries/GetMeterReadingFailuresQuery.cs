using MediatR;
using AutoMapper.QueryableExtensions;
using AutoMapper;
using EnergyDashboard.Domain.Repository;

namespace EnergyDashboard.Application.MeterReadingFailures.Queries;

public class GetMeterReadingFailuresQuery : IRequest<IEnumerable<MeterReadingFailureDto>>
{
}

public class GetMeterReadingFailuresQueryHandler : IRequestHandler<GetMeterReadingFailuresQuery, IEnumerable<MeterReadingFailureDto>>
{
    private readonly IMeterReadingFailureRepository _meterReadingFailureRepository;
    private readonly IMapper _mapper;

    public GetMeterReadingFailuresQueryHandler(IMeterReadingFailureRepository meterReadingFailureRepository, IMapper mapper)
    {
        _meterReadingFailureRepository = meterReadingFailureRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<MeterReadingFailureDto>> Handle(GetMeterReadingFailuresQuery request, CancellationToken cancellationToken)
    {
        var entitys = await _meterReadingFailureRepository.GetAllAsync(cancellationToken);
        return entitys.ProjectTo<MeterReadingFailureDto>(_mapper);
    }
}
