using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using EnergyDashboard.Application.Networks;
using EnergyDashboard.Application.Networks.Queries;
using EnergyDashboard.Application.Networks.Commands;
using MediatR;
using EnergyDashboard.Domain.Diagram;
using EnergyDashboard.Domain.Entities;

#nullable disable

namespace EnergyDashboard.Controllers;

[Route("[controller]")]
[ApiController]

public class NetworksController : ControllerBase
{
    private readonly ISender _mediator;

    public NetworksController(ISender mediator)
    {
        _mediator = mediator;
    }

    // GET: api/Networks
    [HttpGet]
    public async Task<ActionResult<IEnumerable<NetworkDto>>> GetNetworks(CancellationToken token)
    {
        var networks = await _mediator.Send(new GetNetworksQuery(), token);
        return Ok(networks);
    }

    // GET: api/Networks/5
    [HttpGet("{id}")]
    public async Task<ActionResult<DiagramModel>> GetNetwork([FromRoute] Guid id, CancellationToken token)
    {
        var model = await _mediator.Send(new GetNetworkQuery() { Id = id}, token);
        return Ok(model);
    }

    // GET: api/Networks/Export/id
    [HttpGet("Export/{id}")]
    public async Task<ActionResult<Network>> ExportNetwork([FromRoute] Guid id, CancellationToken token)
    {
        var network = await _mediator.Send(new ExportNetworkQuery() { Id = id }, token);
        return Ok(network);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admins")]
    public async Task<ActionResult<DiagramModel>> PutNetwork([FromRoute] Guid id, [FromBody]DiagramModel networkDiagram, CancellationToken token)
    {
        var model = await _mediator.Send(new UpdateNetworkCommand() { Id = id, Diagram = networkDiagram }, token);
        return Ok(model);
    }

    // GET api/Networks/GetDiagramSVG/{id}
    [HttpGet("GetDiagramSVG/{id}")]
    public async Task<IActionResult> GetDiagramSVG([FromRoute]Guid id, CancellationToken token)
    {
        var model = await _mediator.Send(new GetNetworkDiagramQuery() { Id = id }, token);
        return new ObjectResult(model.toSVG());
    }
}
