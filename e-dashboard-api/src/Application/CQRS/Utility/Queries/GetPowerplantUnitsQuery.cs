using MediatR;
using EnergyDashboard.Application.Models;
using EnergyDashboard.Domain.Repository;

namespace EnergyDashboard.Application.Utility.Queries;

public class GetPowerplantUnitsQuery : IRequest<IEnumerable<ValueLabel>>
{
    public int PowerPlantId { get; set; }
}

public class GetPowerplantUnitsQueryHandler : IRequestHandler<GetPowerplantUnitsQuery, IEnumerable<ValueLabel>>
{
    private readonly IEquipmentRepository _equipmentRepository;

    public GetPowerplantUnitsQueryHandler(IEquipmentRepository equipmentRepository)
    {
        _equipmentRepository = equipmentRepository;
    }

    public async Task<IEnumerable<ValueLabel>> Handle(GetPowerplantUnitsQuery request, CancellationToken cancellationToken)
    {
        var powerPlantUnits = await _equipmentRepository.GeneratorFeedersEquipmentAsync(cancellationToken);

        var ret = powerPlantUnits
            .Where(w => w.PowerplantOperatorId == request.PowerPlantId)
            .Select(s => new ValueLabel
            {
                Value = s.Id.ToString(),
                Label = s.Name
            })
            .OrderBy(o => o.Label).ToList();

        return ret;
    }
}
