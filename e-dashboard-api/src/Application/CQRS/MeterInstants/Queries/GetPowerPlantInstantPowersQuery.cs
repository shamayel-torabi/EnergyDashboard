using EnergyDashboard.Application.Interfaces;
using EnergyDashboard.Application.Models.Meters;
using MediatR;

namespace EnergyDashboard.Application.MeterInstants.Queries;

public class GetPowerPlantInstantPowersQuery : IRequest<IEnumerable<PowerPlantInstantPower>>
{
}

public class GetPowerPlantInstantPowersQueryHandler : IRequestHandler<GetPowerPlantInstantPowersQuery, IEnumerable<PowerPlantInstantPower>>
{
	private readonly IMeterInstantService _meterInstantService;

	public GetPowerPlantInstantPowersQueryHandler(IMeterInstantService meterInstantService)
	{
		_meterInstantService = meterInstantService ?? throw new ArgumentNullException(nameof(meterInstantService));
	}

	public async Task<IEnumerable<PowerPlantInstantPower>> Handle(GetPowerPlantInstantPowersQuery request, CancellationToken cancellationToken)
	{
		return await _meterInstantService.GetPowerPlantInstantPowersAsync(cancellationToken);
	}
}

