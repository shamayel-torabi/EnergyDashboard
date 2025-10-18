using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using EnergyDashboard.Application.Areas.Queries;
using EnergyDashboard.Application.Areas.Commands;
using MediatR;
using EnergyDashboard.Domain.Diagram;

#nullable disable

namespace EnergyDashboard.Controllers;

[Route("[controller]")]
[ApiController]

public class AreasController : ControllerBase
{
    private readonly ISender _mediator;

    public AreasController(ISender mediator)
    {
        _mediator = mediator;
    }

    // GET: api/Areas/5
    [HttpGet("{id}")]
    public async Task<ActionResult<DiagramModel>> GetArea([FromRoute] Guid id, CancellationToken token)
    {
        var area = await _mediator.Send(new GetAreaDiagramQuery() { Id = id }, token);
        return Ok(area);
    }

    // PUT: api/Areas/5
    [HttpPut("{id}")]
    [Authorize(Roles = "Admins")]

    public async Task<ActionResult<DiagramModel>> PutArea([FromRoute] Guid id, [FromBody]DiagramModel areaDiagram, CancellationToken token)
    {
        var area = await _mediator.Send(new UpdateAreaCommand() { Id = id, Diagram = areaDiagram}, token);
        return Ok(area);
    }

    // GET api/Areas/GetDiagramSVG/{id}
    [HttpGet("GetDiagramSVG/{id}")]
    public async Task<IActionResult> GetDiagramSVG(Guid id, CancellationToken token)
    {
        var area = await _mediator.Send(new GetAreaDiagramQuery() { Id = id }, token);
        return new ObjectResult(area.toSVG());
    }
}
