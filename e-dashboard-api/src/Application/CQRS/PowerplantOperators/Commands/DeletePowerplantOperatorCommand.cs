using MediatR;
using AutoMapper;
using Application.Common.Exceptions;
using EnergyDashboard.Domain.Entities;
using EnergyDashboard.Domain.Repository;

namespace EnergyDashboard.Application.PowerplantOperators.Commands;

public class DeletePowerplantOperatorCommand : IRequest<PowerplantOperatorDto>
{
    public int Id { get; set; }
}

public class DeletePowerplantOperatorCommandHandler : IRequestHandler<DeletePowerplantOperatorCommand, PowerplantOperatorDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPowerplantOperatorRepository _powerplantOperatorRepository;
    private readonly IMapper _mapper;

    public DeletePowerplantOperatorCommandHandler(IUnitOfWork unitOfWork, IPowerplantOperatorRepository powerplantOperatorRepository, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _powerplantOperatorRepository = powerplantOperatorRepository;
        _mapper = mapper;
    }

    public async Task<PowerplantOperatorDto> Handle(DeletePowerplantOperatorCommand request, CancellationToken cancellationToken)
    {
        var entity = await _powerplantOperatorRepository.GetByIdAsync(request.Id, cancellationToken);

        if (entity is null)
            throw new NotFoundException(nameof(PowerplantOperator), request.Id);

        var ret = _powerplantOperatorRepository.Remove(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
       return _mapper.Map<PowerplantOperatorDto>(ret);
    }
}
