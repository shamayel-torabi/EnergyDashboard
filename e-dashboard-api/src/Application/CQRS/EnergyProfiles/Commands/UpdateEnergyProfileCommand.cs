using MediatR;
using AutoMapper;
using EnergyDashboard.Domain.Entities;
using Application.Common.Exceptions;
using EnergyDashboard.Domain.Repository;

namespace EnergyDashboard.Application.EnergyProfiles.Commands;

public class UpdateEnergyProfileCommand : IRequest
{
    public Guid Id { get; set; }
    public EnergyProfileDto EnergyProfile { get; set; }
}

public class UpdateEnergyProfileCommandHandler : IRequestHandler<UpdateEnergyProfileCommand>
{
    private readonly IEnergyProfileRepository _energyProfileRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UpdateEnergyProfileCommandHandler(IEnergyProfileRepository energyProfileRepository, IUnitOfWork unitOfWork, IMapper mapper)
    {
        _energyProfileRepository = energyProfileRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task Handle(UpdateEnergyProfileCommand request, CancellationToken cancellationToken)
    {
        var entity = await _energyProfileRepository.GetByIdAsync(request.Id, cancellationToken);

        if (entity is null)
        {
            throw new NotFoundException(nameof(EnergyProfile), request.Id);
        }

        entity.RecordDate = request.EnergyProfile.RecordDate;
        entity.ImportWatt = request.EnergyProfile.ImportWatt;
        entity.ImportVar = request.EnergyProfile.ImportVar;
        entity.ExportWatt = request.EnergyProfile.ExportWatt;
        entity.ExportVar = request.EnergyProfile.ExportVar;
        entity.ImportTotalWatt = request.EnergyProfile.ImportTotalWatt;
        entity.ExportTotalWatt = request.EnergyProfile.ExportTotalWatt;
        entity.EquipmentId = request.EnergyProfile.EquipmentId;

        _energyProfileRepository.Update(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
