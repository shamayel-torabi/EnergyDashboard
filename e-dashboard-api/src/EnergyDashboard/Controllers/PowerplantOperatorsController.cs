using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using EnergyDashboard.Application.PowerplantOperators;
using EnergyDashboard.Application.PowerplantOperators.Queries;
using EnergyDashboard.Application.PowerplantOperators.Commands;
using MediatR;

namespace EnergyDashboard.Controllers;

[Route("[controller]")]
[ApiController]

public class PowerplantOperatorsController : ControllerBase
{
    private readonly ISender _mediator;

    public PowerplantOperatorsController(ISender mediator)
    {
        _mediator = mediator;
    }

    // GET: api/PowerplantOperators
    [HttpGet]
    public async Task<ActionResult<IEnumerable<PowerplantOperatorDto>>> GetPowerplantOperators(CancellationToken token)
    {
        var powerplantOperators = await _mediator.Send(new GetPowerplantOperatorsQuery(), token);
        return Ok(powerplantOperators);
    }

    // GET: api/PowerplantOperators/5
    [HttpGet("{id}")]
    public async Task<ActionResult<PowerplantOperatorDto>> GetPowerplantOperator(int id, CancellationToken token)
    {
        var powerplantOperator = await _mediator.Send(new GetPowerplantOperatorQuery() { Id = id}, token);
        return Ok(powerplantOperator);
    }

    // PUT: api/PowerplantOperators/5
    [HttpPut("{id}")]
    [Authorize(Roles = "Admins")]
    public async Task<IActionResult> PutPowerplantOperator(int id, PowerplantOperatorDto powerplantOperator, CancellationToken token)
    {
        if (id != powerplantOperator.Id)
        {
            return BadRequest();
        }

        await _mediator.Send(new UpdatePowerplantOperatorCommand() { Id = id, Name = powerplantOperator.Name }, token);
        return NoContent();
    }

    // POST: api/PowerplantOperators
    [HttpPost]
    [Authorize(Roles = "Admins")]
    public async Task<ActionResult<PowerplantOperatorDto>> PostPowerplantOperator(PowerplantOperatorDto powerplantOperator, CancellationToken token)
    {
        var ppo = await _mediator.Send(new CreatePowerplantOperatorCommand() { Name = powerplantOperator.Name }, token);
        return Ok(ppo);
    }

    // DELETE: api/PowerplantOperators/5
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admins")]
    public async Task<ActionResult<PowerplantOperatorDto>> DeletePowerplantOperator(int id, CancellationToken token)
    {
        var powerplantOperator = await _mediator.Send(new DeletePowerplantOperatorCommand() { Id = id }, token);
        return Ok(powerplantOperator);
    }
}
