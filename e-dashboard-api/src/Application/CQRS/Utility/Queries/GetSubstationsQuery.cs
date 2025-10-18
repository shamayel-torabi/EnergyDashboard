using MediatR;
using EnergyDashboard.Domain.Repository;
using EnergyDashboard.Application.Models;

namespace EnergyDashboard.Application.Utility.Queries;

public class GetSubstationsQuery : IRequest<IEnumerable<ValueLabel>>
{
}

public class GetSubstationsQueryHandler : IRequestHandler<GetSubstationsQuery, IEnumerable<ValueLabel>>
{
    private readonly ISubstationRepository _substationRepository;

    public GetSubstationsQueryHandler(ISubstationRepository substationRepository)
    {
        _substationRepository = substationRepository;
    }

    public async Task<IEnumerable<ValueLabel>> Handle(GetSubstationsQuery request, CancellationToken cancellationToken)
    {
        var substations = await _substationRepository.GetAllAsync(cancellationToken);
        var ret = substations
            .Select(s => new ValueLabel { Label = s.Name, Value = s.Id.ToString() })
            .OrderBy(o => o.Label);
        return ret;
    }
}
