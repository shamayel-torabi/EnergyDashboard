using MediatR;
using EnergyDashboard.Domain.Entities;
using AutoMapper;
using EnergyDashboard.Domain.Repository;

namespace EnergyDashboard.Application.PowerplantOperators.Commands;

public class CreatePowerplantOperatorCommand : IRequest<PowerplantOperatorDto>
{
    public string Name { get; set; }
}

public class CreatePowerplantOperatorCommandHandler : IRequestHandler<CreatePowerplantOperatorCommand, PowerplantOperatorDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPowerplantOperatorRepository _powerplantOperatorRepository;
    private readonly IMapper _mapper;

    public CreatePowerplantOperatorCommandHandler(IUnitOfWork unitOfWork, IPowerplantOperatorRepository powerplantOperatorRepository, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _powerplantOperatorRepository = powerplantOperatorRepository;
        _mapper = mapper;
    }

    public async Task<PowerplantOperatorDto> Handle(CreatePowerplantOperatorCommand request, CancellationToken cancellationToken)
    {
        var entity = new PowerplantOperator
        { 
            Name = request.Name,
        };

        var ret = _powerplantOperatorRepository.Add(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return _mapper.Map<PowerplantOperatorDto>(ret);
    }
}
