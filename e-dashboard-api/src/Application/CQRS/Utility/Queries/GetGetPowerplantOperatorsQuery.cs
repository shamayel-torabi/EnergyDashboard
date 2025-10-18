using MediatR;
using EnergyDashboard.Application.Models;
using EnergyDashboard.Domain.Repository;

namespace EnergyDashboard.Application.Utility.Queries;

public class GetGetPowerplantOperatorsQuery : IRequest<IEnumerable<ValueLabel>>
{
}

public class GetGetPowerplantOperatorsQueryHandler : IRequestHandler<GetGetPowerplantOperatorsQuery, IEnumerable<ValueLabel>>
{
    private readonly IPowerplantOperatorRepository _powerplantOperatorRepository;

    public GetGetPowerplantOperatorsQueryHandler(IPowerplantOperatorRepository powerplantOperatorRepository)
    {
        _powerplantOperatorRepository = powerplantOperatorRepository;
    }

    public async Task<IEnumerable<ValueLabel>> Handle(GetGetPowerplantOperatorsQuery request, CancellationToken cancellationToken)
    {
        var ppos = await _powerplantOperatorRepository.GetAllAsync(cancellationToken);
        return ppos.Select(x => new ValueLabel { Value = x.Id.ToString(), Label = x.Name });
    }
}
