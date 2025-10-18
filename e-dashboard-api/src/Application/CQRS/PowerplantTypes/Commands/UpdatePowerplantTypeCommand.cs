
using Application.Common.Exceptions;
using EnergyDashboard.Domain.Entities;
using EnergyDashboard.Domain.Repository;
using AutoMapper;
using MediatR;

namespace EnergyDashboard.Application.PowerplantTypes.Commands;

public class UpdatePowerplantTypeCommand : IRequest
{
    public int Id { get; set; }
    public string Name { get; set; }
}

public class UpdatePowerplantTypeCommandHandler : IRequestHandler<UpdatePowerplantTypeCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPowerplantTypeRepository _powerplantTypeRepository;
    private readonly IMapper _mapper;

    public UpdatePowerplantTypeCommandHandler(IUnitOfWork unitOfWork, IPowerplantTypeRepository powerplantTypeRepository, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _powerplantTypeRepository = powerplantTypeRepository;
        _mapper = mapper;
    }

    public async Task Handle(UpdatePowerplantTypeCommand request, CancellationToken cancellationToken)
    {
        var entity = await _powerplantTypeRepository.GetByIdAsync(request.Id, cancellationToken);

        if (entity is null)
        {
            throw new NotFoundException(nameof(PowerplantOperator), request.Id);
        }

        entity.Name = request.Name;

        _powerplantTypeRepository.Update(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
