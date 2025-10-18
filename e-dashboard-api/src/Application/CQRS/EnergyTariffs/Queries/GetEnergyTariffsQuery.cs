using MediatR;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using EnergyDashboard.Domain.Repository;

namespace EnergyDashboard.Application.EnergyTariffs.Queries;

public class GetEnergyTariffsQuery : IRequest<IEnumerable<EnergyTariffDto>>
{
}

public class GetEnergyTariffsQueryHandler : IRequestHandler<GetEnergyTariffsQuery, IEnumerable<EnergyTariffDto>>
{
    private readonly IEnergyTariffRepository _context;
    private readonly IMapper _mapper;

    public GetEnergyTariffsQueryHandler(IEnergyTariffRepository context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IEnumerable<EnergyTariffDto>> Handle(GetEnergyTariffsQuery request, CancellationToken cancellationToken)
    {
        var entitys = await _context.GetAllAsync(cancellationToken);
        return entitys.ProjectTo<EnergyTariffDto>(_mapper);
    }
}
