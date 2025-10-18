using Application.Common.Exceptions;
using EnergyDashboard.Domain.Entities;
using EnergyDashboard.Domain.Repository;
using EnergyDashboard.Application.Models;
using MediatR;

namespace EnergyDashboard.Application.Substations.Queries;

public class GetSubstationEquipmentsQuery : IRequest<IEnumerable<ValueLabel>>
{
    public Guid SubstationId { get; set; }
}

public class GetSubstationEquipmentsQueryHandler : IRequestHandler<GetSubstationEquipmentsQuery, IEnumerable<ValueLabel>>
{
    private readonly ISubstationRepository _substationRepository;

    public GetSubstationEquipmentsQueryHandler(ISubstationRepository substationRepository)
    {
        _substationRepository = substationRepository;
    }

    public async Task<IEnumerable<ValueLabel>> Handle(GetSubstationEquipmentsQuery request, CancellationToken cancellationToken)
    {
        var substation = await _substationRepository.GetSubstationWithEquipments(request.SubstationId, cancellationToken);

        if (substation is null)
            throw new NotFoundException(nameof(Substation), request.SubstationId);

        var ret = substation.Equipments
            .Select(s => new ValueLabel { Value = s.Id.ToString(), Label = s.Name })
            .OrderBy(o => o.Label)
            .ToList();

        return ret;
    }
}

