using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using EnergyDashboard.Application.Zones.Queries;
using EnergyDashboard.Application.Zones.Commands;
using EnergyDashboard.Domain.Diagram;
using MediatR;

#nullable disable

namespace EnergyDashboard.Controllers;

[Route("[controller]")]
[ApiController]

public class ZonesController : ControllerBase
{
    private readonly ISender _mediator;

    public ZonesController(ISender mediator)
    {
        _mediator = mediator;
    }

    // GET: api/Zones/5
    [HttpGet("{id}")]
    public async Task<ActionResult<DiagramModel>> GetZone(Guid id, CancellationToken token)
    {
        var zone = await _mediator.Send(new GetZoneDiagramQuery() { Id = id }, token);
        return Ok(zone);
    }

    // PUT: api/Zones/5
    [HttpPut("{id}")]
    [Authorize(Roles = "Admins")]

    public async Task<ActionResult<DiagramModel>> PutZone([FromRoute] Guid id, [FromBody]DiagramModel zoneDiagram, CancellationToken token)
    {
        var zone = await _mediator.Send(new UpdateZoneCommand() { Id = id, Diagram = zoneDiagram }, token);
        return Ok(zone);
    }

    // GET api/Zones/GetDiagramSVG/{id}
    [HttpGet("GetDiagramSVG/{id}")]
    public async Task<IActionResult> GetDiagramSVG(Guid id, CancellationToken token)
    {
        var zone = await _mediator.Send(new GetZoneDiagramQuery() { Id = id }, token);
        return new ObjectResult(zone.toSVG());
    }
}
