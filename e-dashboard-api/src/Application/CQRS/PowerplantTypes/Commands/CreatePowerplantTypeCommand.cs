using EnergyDashboard.Domain.Entities;
using EnergyDashboard.Domain.Repository;
using AutoMapper;
using MediatR;

namespace EnergyDashboard.Application.PowerplantTypes.Commands;

public class CreatePowerplantTypeCommand : IRequest<PowerplantTypeDto>
{
    public string Name { get; set; }
}

public class CreatePowerplantTypeCommandHandler : IRequestHandler<CreatePowerplantTypeCommand, PowerplantTypeDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPowerplantTypeRepository _powerplantTypeRepository;
    private readonly IMapper _mapper;

    public CreatePowerplantTypeCommandHandler(IUnitOfWork unitOfWork, IPowerplantTypeRepository powerplantTypeRepository, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _powerplantTypeRepository = powerplantTypeRepository;
        _mapper = mapper;
    }

    public async Task<PowerplantTypeDto> Handle(CreatePowerplantTypeCommand request, CancellationToken cancellationToken)
    {
        var entity = new PowerplantType
        { 
            Name = request.Name,
        };

        var ret = _powerplantTypeRepository.Add(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return _mapper.Map<PowerplantTypeDto>(ret);
    }
}
