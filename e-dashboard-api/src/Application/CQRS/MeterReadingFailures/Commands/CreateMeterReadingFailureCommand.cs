using MediatR;
using AutoMapper;
using EnergyDashboard.Domain.Entities;
using EnergyDashboard.Domain.Repository;

namespace EnergyDashboard.Application.MeterReadingFailures.Commands;

public class CreateMeterReadingFailureCommand : IRequest<MeterReadingFailureDto>
{
    public MeterReadingFailureDto MeterReadingFailure { get; set; }
}

public class CreateMeterReadingFailureCommandHandler : IRequestHandler<CreateMeterReadingFailureCommand, MeterReadingFailureDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMeterReadingFailureRepository _meterReadingFailureRepository;
    private readonly IMapper _mapper;

    public CreateMeterReadingFailureCommandHandler(
        IUnitOfWork unitOfWork,
        IMeterReadingFailureRepository meterReadingFailureRepository,
        IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _meterReadingFailureRepository = meterReadingFailureRepository;
        _mapper = mapper;
    }

    public async Task<MeterReadingFailureDto> Handle(CreateMeterReadingFailureCommand request, CancellationToken cancellationToken)
    {
        var entity = new MeterReadingFailure(request.MeterReadingFailure.Id)
        {
            RecordDate = request.MeterReadingFailure.RecordDate,
            EquipmentId = request.MeterReadingFailure.EquipmentId,
        };

        _meterReadingFailureRepository.Add(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return _mapper.Map<MeterReadingFailureDto>(entity);
    }
}
