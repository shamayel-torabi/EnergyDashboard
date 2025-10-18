using MediatR;
using AutoMapper;
using Application.Common.Exceptions;
using EnergyDashboard.Domain.Entities;
using EnergyDashboard.Domain.Repository;

namespace EnergyDashboard.Application.EnergyTariffs.Queries;

public class GetEnergyTariffQuery : IRequest<EnergyTariffDto>
{
    public Guid Id { get; set; }
}

public class GetEnergyTariffQueryHandler : IRequestHandler<GetEnergyTariffQuery, EnergyTariffDto>
{
    private readonly IEnergyTariffRepository _context;
    private readonly IMapper _mapper;

    public GetEnergyTariffQueryHandler(IEnergyTariffRepository context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<EnergyTariffDto> Handle(GetEnergyTariffQuery request, CancellationToken cancellationToken)
    {
        var entity = await _context.GetByIdAsync(request.Id, cancellationToken);

        if (entity is null)
            throw new NotFoundException(nameof(EnergyTariff), request.Id);

        return _mapper.Map<EnergyTariffDto>(entity);
    }
}
