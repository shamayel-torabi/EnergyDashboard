using MediatR;
using EnergyDashboard.Application.Models;
using EnergyDashboard.Domain.Repository;

namespace EnergyDashboard.Application.Utility.Queries;

public class GetPowerplantTypesQuery : IRequest<IEnumerable<ValueLabel>>
{
}

public class GetPowerplantTypesQueryHandler : IRequestHandler<GetPowerplantTypesQuery, IEnumerable<ValueLabel>>
{
    private readonly IPowerplantTypeRepository _powerplantTypeRepository;

    public GetPowerplantTypesQueryHandler(IPowerplantTypeRepository powerplantTypeRepository)
    {
        _powerplantTypeRepository = powerplantTypeRepository;
    }

    public async Task<IEnumerable<ValueLabel>> Handle(GetPowerplantTypesQuery request, CancellationToken cancellationToken)
    {
        var ppts = await _powerplantTypeRepository.GetAllAsync(cancellationToken);
        return ppts.Select(x => new ValueLabel { Value = x.Id.ToString(), Label = x.Name });
    }
}
