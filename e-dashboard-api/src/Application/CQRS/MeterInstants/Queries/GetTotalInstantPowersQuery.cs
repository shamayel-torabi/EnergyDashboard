using EnergyDashboard.Application.Interfaces;
using EnergyDashboard.Application.Models.Meters;
using MediatR;

namespace EnergyDashboard.Application.MeterInstants.Queries;

public class GetTotalInstantPowersQuery : IRequest<TotalInstantPower>
{
}

public class GetTotalInstantPowersQueryHandler : IRequestHandler<GetTotalInstantPowersQuery, TotalInstantPower>
{
	private readonly IMeterInstantService _meterInstantService;

	public GetTotalInstantPowersQueryHandler(IMeterInstantService meterInstantService)
	{
		_meterInstantService = meterInstantService ?? throw new ArgumentNullException(nameof(meterInstantService));
	}

	public async Task<TotalInstantPower> Handle(GetTotalInstantPowersQuery request, CancellationToken cancellationToken)
	{
		return await _meterInstantService.GetTotalInstantPowersAsync(cancellationToken);
	}
}
