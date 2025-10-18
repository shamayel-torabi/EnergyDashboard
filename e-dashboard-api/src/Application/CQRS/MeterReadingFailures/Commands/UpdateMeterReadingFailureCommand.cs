using MediatR;
using AutoMapper;
using EnergyDashboard.Domain.Entities;
using Application.Common.Exceptions;
using EnergyDashboard.Domain.Repository;

namespace EnergyDashboard.Application.MeterReadingFailures.Commands;

public class UpdateMeterReadingFailureCommand : IRequest
{
    public Guid Id { get; set; }
    public MeterReadingFailureDto MeterReadingFailure { get; set; }
}

public class UpdateMeterReadingFailureCommandHandler : IRequestHandler<UpdateMeterReadingFailureCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMeterReadingFailureRepository _meterReadingFailureRepository;
    private readonly IMapper _mapper;

    public UpdateMeterReadingFailureCommandHandler(
        IUnitOfWork unitOfWork,
        IMeterReadingFailureRepository meterReadingFailureRepository,
        IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _meterReadingFailureRepository = meterReadingFailureRepository;
        _mapper = mapper;
    }

    public async Task Handle(UpdateMeterReadingFailureCommand request, CancellationToken cancellationToken)
    {
        var entity = await _meterReadingFailureRepository.GetByIdAsync(request.Id, cancellationToken);

        if (entity is null)
            throw new NotFoundException(nameof(MeterReadingFailure), request.Id);

        entity.RecordDate = request.MeterReadingFailure.RecordDate;
        entity.EquipmentId = request.MeterReadingFailure.EquipmentId;

        _meterReadingFailureRepository.Update(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
