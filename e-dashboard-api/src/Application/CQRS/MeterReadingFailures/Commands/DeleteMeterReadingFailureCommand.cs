using MediatR;
using AutoMapper;
using Application.Common.Exceptions;
using EnergyDashboard.Domain.Entities;
using EnergyDashboard.Domain.Repository;

namespace EnergyDashboard.Application.MeterReadingFailures.Commands;

public class DeleteMeterReadingFailureCommand : IRequest<MeterReadingFailureDto>
{
    public Guid Id { get; set; }
}

public class DeleteMeterReadingFailureCommandHandler : IRequestHandler<DeleteMeterReadingFailureCommand, MeterReadingFailureDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMeterReadingFailureRepository _meterReadingFailureRepository;
    private readonly IMapper _mapper;

    public DeleteMeterReadingFailureCommandHandler(
        IUnitOfWork unitOfWork,
        IMeterReadingFailureRepository meterReadingFailureRepository,
        IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _meterReadingFailureRepository = meterReadingFailureRepository;
        _mapper = mapper;
    }

    public async Task<MeterReadingFailureDto> Handle(DeleteMeterReadingFailureCommand request, CancellationToken cancellationToken)
    {
        var entity = await _meterReadingFailureRepository.GetByIdAsync(request.Id, cancellationToken);

        if (entity is null)
            throw new NotFoundException(nameof(MeterReadingFailure), request.Id);

        _meterReadingFailureRepository.Remove(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        var ret = _mapper.Map<MeterReadingFailureDto>(entity);
        return ret;
    }
}
