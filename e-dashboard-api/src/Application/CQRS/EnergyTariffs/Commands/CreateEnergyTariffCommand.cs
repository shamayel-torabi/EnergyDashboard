using MediatR;
using EnergyDashboard.Domain.Entities;
using EnergyDashboard.Domain.Repository;
using AutoMapper;

namespace EnergyDashboard.Application.EnergyTariffs.Commands;

public class CreateEnergyTariffCommand : IRequest<EnergyTariffDto>
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public IEnumerable<EnergyTariffRate> EnergyTariffRates { get; set; }
}

public class CreateEnergyTariffsCommandHandler : IRequestHandler<CreateEnergyTariffCommand, EnergyTariffDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEnergyTariffRepository _context;
    private readonly IMapper _mapper;

    public CreateEnergyTariffsCommandHandler(IUnitOfWork unitOfWork, IEnergyTariffRepository context, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _context = context;
        _mapper = mapper;
    }

    public async Task<EnergyTariffDto> Handle(CreateEnergyTariffCommand request, CancellationToken cancellationToken)
    {
        var entity = EnergyTariff.Create(
            request.Id,
            request.Name,
            request.StartDate,
            request.EndDate,
            request.EnergyTariffRates);

        var ret = _context.Add(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return _mapper.Map<EnergyTariffDto>(ret);
    }
}
