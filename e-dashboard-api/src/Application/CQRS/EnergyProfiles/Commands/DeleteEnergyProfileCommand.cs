using MediatR;
using AutoMapper;
using EnergyDashboard.Domain.Entities;
using Application.Common.Exceptions;
using EnergyDashboard.Domain.Repository;

namespace EnergyDashboard.Application.EnergyProfiles.Commands;

public class DeleteEnergyProfileCommand : IRequest<EnergyProfileDto>
{
    public Guid Id { get; set; }
}

public class DeleteEnergyProfileCommandHandler : IRequestHandler<DeleteEnergyProfileCommand, EnergyProfileDto>
{
    private readonly IEnergyProfileRepository _energyProfileRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public DeleteEnergyProfileCommandHandler(IEnergyProfileRepository energyProfileRepository, IUnitOfWork unitOfWork, IMapper mapper)
    {
        _energyProfileRepository = energyProfileRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<EnergyProfileDto> Handle(DeleteEnergyProfileCommand request, CancellationToken cancellationToken)
    {
        var entity = await _energyProfileRepository.GetByIdAsync(request.Id , cancellationToken);

        if (entity is null)
        {
            throw new NotFoundException(nameof(EnergyProfile), request.Id);
        }

        _energyProfileRepository.Remove(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        var ret = _mapper.Map<EnergyProfileDto>(entity);
        return ret;
    }
}
