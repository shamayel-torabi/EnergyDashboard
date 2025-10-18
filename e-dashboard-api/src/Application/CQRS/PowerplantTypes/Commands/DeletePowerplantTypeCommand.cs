
using Application.Common.Exceptions;
using EnergyDashboard.Domain.Entities;
using EnergyDashboard.Domain.Repository;
using AutoMapper;
using MediatR;

namespace EnergyDashboard.Application.PowerplantTypes.Commands;

public class DeletePowerplantTypeCommand : IRequest<PowerplantTypeDto>
{
    public int Id { get; set; }
}

public class DeletePowerplantTypeCommandHandler : IRequestHandler<DeletePowerplantTypeCommand, PowerplantTypeDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPowerplantTypeRepository _powerplantTypeRepository;
    private readonly IMapper _mapper;

    public DeletePowerplantTypeCommandHandler(IUnitOfWork unitOfWork, IPowerplantTypeRepository powerplantTypeRepository, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _powerplantTypeRepository = powerplantTypeRepository;
        _mapper = mapper;
    }

    public async Task<PowerplantTypeDto> Handle(DeletePowerplantTypeCommand request, CancellationToken cancellationToken)
    {
        var entity = await _powerplantTypeRepository.GetByIdAsync(request.Id, cancellationToken);

        if (entity is null)
            throw new NotFoundException(nameof(PowerplantOperator), request.Id);

        var ret = _powerplantTypeRepository.Remove(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return _mapper.Map<PowerplantTypeDto>(ret);
    }
}
