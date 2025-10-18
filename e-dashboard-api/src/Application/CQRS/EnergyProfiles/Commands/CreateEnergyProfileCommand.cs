using MediatR;
using AutoMapper;
using EnergyDashboard.Domain.Entities;
using EnergyDashboard.Domain.Repository;

namespace EnergyDashboard.Application.EnergyProfiles.Commands;

public class CreateEnergyProfileCommand : IRequest<EnergyProfileDto>
{
    public EnergyProfileDto EnergyProfile { get; set; }
}

public class CreateEnergyProfileCommandHandler : IRequestHandler<CreateEnergyProfileCommand, EnergyProfileDto>
{
    private readonly IEnergyProfileRepository _energyProfileRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateEnergyProfileCommandHandler(IEnergyProfileRepository energyProfileRepository, IUnitOfWork unitOfWork, IMapper mapper)
    {
        _energyProfileRepository = energyProfileRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<EnergyProfileDto> Handle(CreateEnergyProfileCommand request, CancellationToken cancellationToken)
    {
        var entity = new EnergyProfile(request.EnergyProfile.Id)
        {
            RecordDate = request.EnergyProfile.RecordDate,
            ImportWatt = request.EnergyProfile.ImportWatt,
            ImportVar = request.EnergyProfile.ImportVar,
            ExportWatt = request.EnergyProfile.ExportWatt,
            ExportVar = request.EnergyProfile.ExportVar,
            ImportTotalWatt = request.EnergyProfile.ImportTotalWatt,
            ExportTotalWatt = request.EnergyProfile.ExportTotalWatt,
            EquipmentId = request.EnergyProfile.EquipmentId,
        };

        _energyProfileRepository.Add(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        var ret = _mapper.Map<EnergyProfileDto>(entity);
        return ret;
    }
}
