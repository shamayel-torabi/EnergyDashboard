using Microsoft.AspNetCore.Mvc;
using EnergyDashboard.Application.MeterInstants.Queries;
using EnergyDashboard.Application.Models.Meters;
using MediatR;

namespace EnergyDashboard.Controllers;

[Route("[controller]")]
[ApiController]
public class MeterInstantsController : ControllerBase
{
	private readonly ISender _mediator;

	public MeterInstantsController(ISender mediator)
	{
		_mediator = mediator ?? throw new ArgumentNullException("mediator");
	}


	[HttpGet("GetPowerPlantInstantPowers")]
	public async Task<ActionResult<Task<IEnumerable<PowerPlantInstantPower>>>> GetPowerPlantInstantPowers(CancellationToken token)
	{
		GetPowerPlantInstantPowersQuery query = new GetPowerPlantInstantPowersQuery();
		var result = await _mediator.Send(query, token);
		return Ok(result);
	}

	[HttpGet("GetTotalInstantPowers")]
	public async Task<ActionResult<Task<TotalInstantPower>>> GetTotalInstantPowers(CancellationToken token)
	{
		GetTotalInstantPowersQuery query = new GetTotalInstantPowersQuery();
		var result = await _mediator.Send(query, token);
		return Ok(result);
	}
}
