using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using EnergyDashboard.Application.LoadFeederTypes;
using EnergyDashboard.Application.LoadFeederTypes.Queries;
using EnergyDashboard.Application.LoadFeederTypes.Commands;
using MediatR;

namespace EnergyDashboard.Controllers;

[Route("[controller]")]
[ApiController]

public class LoadFeederTypesController : ControllerBase
{
    private readonly ISender _mediator;

    public LoadFeederTypesController(ISender mediator)
    {
        _mediator = mediator;
    }

    // GET: api/LoadFeederTypes
    [HttpGet]
    public async Task<ActionResult<IEnumerable<LoadFeederTypeDto>>> GetLoadFeederTypes(CancellationToken token)
    {
        var loadFeederTypes = await _mediator.Send(new GetLoadFeederTypesQuery(), token);
        return Ok(loadFeederTypes);
    }

    // GET: api/LoadFeederTypes/5
    [HttpGet("{id}")]
    public async Task<ActionResult<LoadFeederTypeDto>> GetLoadFeederType(int id, CancellationToken token)
    {
        var loadFeederType = await _mediator.Send(new GetLoadFeederTypeQuery() {Id = id }, token);
        return Ok(loadFeederType);
    }

    // PUT: api/LoadFeederTypes/5
    [HttpPut("{id}")]
    [Authorize(Roles = "Admins")]
    public async Task<IActionResult> PutLoadFeederType(int id, LoadFeederTypeDto loadFeederType, CancellationToken token)
    {
        if (id != loadFeederType.Id)
        {
            return BadRequest();
        }

        var command = new UpdateLoadFeederTypeCommand()
        {
            Id = id,
            Name = loadFeederType.Name,
            Type = loadFeederType.Type
        };

        await _mediator.Send(command, token);
        return NoContent();
    }

    // POST: api/LoadFeederTypes
    [HttpPost]
    //[Authorize(Roles = "Admins")]
    public async Task<ActionResult<LoadFeederTypeDto>> PostLoadFeederType(LoadFeederTypeDto loadFeederType, CancellationToken token)
    {
        var command = new CreateLoadFeederTypeCommand() {
            Name = loadFeederType.Name,
            Type = loadFeederType.Type
        };
        var lft = await _mediator.Send(command, token);
        return Ok(lft);
    }

    // DELETE: api/LoadFeederTypes/5
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admins")]
    public async Task<ActionResult<LoadFeederTypeDto>> DeleteLoadFeederType(int id, CancellationToken token)
    {
        var loadFeederType = await _mediator.Send(new DeleteLoadFeederTypeCommand() { Id = id }, token);
        return Ok(loadFeederType);
    }
}
