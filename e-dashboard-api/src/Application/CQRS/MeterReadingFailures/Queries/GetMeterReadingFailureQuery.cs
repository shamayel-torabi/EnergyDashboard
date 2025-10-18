using MediatR;
using AutoMapper;
using Application.Common.Exceptions;
using EnergyDashboard.Domain.Entities;
using EnergyDashboard.Domain.Repository;

namespace EnergyDashboard.Application.MeterReadingFailures.Queries;

public class GetMeterReadingFailureQuery : IRequest<MeterReadingFailureDto>
{
    public Guid Id { get; set; }
}

public class GetMeterReadingFailureQueryHandler : IRequestHandler<GetMeterReadingFailureQuery, MeterReadingFailureDto>
{
    private readonly IMeterReadingFailureRepository _meterReadingFailureRepository;
    private readonly IMapper _mapper;

    public GetMeterReadingFailureQueryHandler(IMeterReadingFailureRepository meterReadingFailureRepository, IMapper mapper)
    {
        _meterReadingFailureRepository = meterReadingFailureRepository;
        _mapper = mapper;
    }

    public async Task<MeterReadingFailureDto> Handle(GetMeterReadingFailureQuery request, CancellationToken cancellationToken)
    {
        var entity = await _meterReadingFailureRepository.GetByIdAsync(request.Id , cancellationToken);

        if (entity is null)
            throw new NotFoundException(nameof(MeterReadingFailure), request.Id);

        var ret = _mapper.Map<MeterReadingFailureDto>(entity);
        return ret;
    }
}
