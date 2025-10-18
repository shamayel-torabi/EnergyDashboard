using MediatR;
using AutoMapper;
using EnergyDashboard.Domain.Repository;

namespace EnergyDashboard.Application.EnergyProfiles.Queries;

public class GetEquipmentEnergyProfileIntervalQuery : IRequest<IEnumerable<EnergyProfileDto>>
{
    public Guid EquipmentId { get; set; }
    public DateTimeOffset StartDate { get; set; }
    public DateTimeOffset EndDate { get; set; }
}

public class GetEquipmentEnergyProfileIntervalQueryHandler : IRequestHandler<GetEquipmentEnergyProfileIntervalQuery, IEnumerable<EnergyProfileDto>>
{
    private readonly IEnergyProfileRepository _energyProfileRepository;
    private readonly IMapper _mapper;

    public GetEquipmentEnergyProfileIntervalQueryHandler(IEnergyProfileRepository energyProfileRepository, IMapper mapper)
    {
        _energyProfileRepository = energyProfileRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<EnergyProfileDto>> Handle(GetEquipmentEnergyProfileIntervalQuery request, CancellationToken cancellationToken)
    {
        var energyProfile = await _energyProfileRepository.GetEquipmentEnergyProfileIntervalAsync(request.EquipmentId,request.StartDate,request.EndDate, cancellationToken);
        var ret = _mapper.Map<IEnumerable<EnergyProfileDto>>(energyProfile);
        return ret;
    }
}
