using Application.Common.Exceptions;
using EnergyDashboard.Domain.Entities;
using EnergyDashboard.Domain.Repository;
using MediatR;

namespace EnergyDashboard.Application.Substations.Queries;

public class GetSubstationNameQuery : IRequest<String>
{
    public Guid Id { get; set; }
}

public class GetSubstationNameQueryHandler : IRequestHandler<GetSubstationNameQuery, String>
{
    private readonly ISubstationRepository _substationRepository;

    public GetSubstationNameQueryHandler(ISubstationRepository substationRepository)
    {
        _substationRepository = substationRepository;
    }

    public async Task<String> Handle(GetSubstationNameQuery request, CancellationToken cancellationToken)
    {
        var substation = await _substationRepository.GetByIdAsync(request.Id , cancellationToken);

        if (substation is null)
            throw new NotFoundException(nameof(Substation), request.Id);

        return substation.Name;
    }
}
