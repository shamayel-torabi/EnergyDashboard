using MediatR;
using AutoMapper;
using EnergyDashboard.Domain.Repository;

namespace EnergyDashboard.Application.EnergyProfiles.Queries;

public class GetEquipmentEnergyProfileQuery : IRequest<IEnumerable<EnergyProfileDto>>
{
    public Guid EquipmentId { get; set; }
    public int Year { get; set; }
    public int Month { get; set; }  
}

public class GetEquipmentEnergyProfileQueryHandler : IRequestHandler<GetEquipmentEnergyProfileQuery, IEnumerable<EnergyProfileDto>>
{
    private readonly IEnergyProfileRepository _energyProfileRepository;
    private readonly IMapper _mapper;

    public GetEquipmentEnergyProfileQueryHandler(IEnergyProfileRepository energyProfileRepository, IMapper mapper)
    {
        _energyProfileRepository = energyProfileRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<EnergyProfileDto>> Handle(GetEquipmentEnergyProfileQuery request, CancellationToken cancellationToken)
    {
        var energyProfile = await _energyProfileRepository.GetEquipmentEnergyMonthlyAsync(request.EquipmentId, request.Year, request.Month, cancellationToken);

        var ret = _mapper.Map<IEnumerable<EnergyProfileDto>>(energyProfile);
        return ret;
    }
}
