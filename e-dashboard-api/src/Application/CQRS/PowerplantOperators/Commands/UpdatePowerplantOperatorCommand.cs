using MediatR;
using AutoMapper;
using EnergyDashboard.Domain.Entities;
using Application.Common.Exceptions;
using EnergyDashboard.Domain.Repository;

namespace EnergyDashboard.Application.PowerplantOperators.Commands;

public class UpdatePowerplantOperatorCommand : IRequest
{
    public int Id { get; set; }
    public string Name { get; set; }
}

public class UpdateMeterTypeCommandHandler : IRequestHandler<UpdatePowerplantOperatorCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPowerplantOperatorRepository _powerplantOperatorRepository;
    private readonly IMapper _mapper;

    public UpdateMeterTypeCommandHandler(IUnitOfWork unitOfWork, IPowerplantOperatorRepository powerplantOperatorRepository, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _powerplantOperatorRepository = powerplantOperatorRepository;
        _mapper = mapper;
    }

    public async Task Handle(UpdatePowerplantOperatorCommand request, CancellationToken cancellationToken)
    {
        var entity = await _powerplantOperatorRepository.GetByIdAsync(request.Id, cancellationToken);

        if (entity is null)
        {
            throw new NotFoundException(nameof(PowerplantOperator), request.Id);
        }

        entity.Name = request.Name;

        _powerplantOperatorRepository.Update(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
