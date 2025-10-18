using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using EnergyDashboard.Application.Substations.Queries;
using EnergyDashboard.Application.Substations.Commands;
using EnergyDashboard.Application.Substations;
using EnergyDashboard.Domain.Diagram;
using MediatR;
using EnergyDashboard.Application.Models;

#nullable disable

namespace EnergyDashboard.Controllers;

[Route("[controller]")]
[ApiController]

public class SubstationsController : Controller
{
    private readonly ISender _mediator;

    public SubstationsController(ISender mediator)
    {
        _mediator = mediator;
    }

    // GET api/Substations/5
    [HttpGet("{id}")]
    public async Task<ActionResult<DiagramModel>> GetSubstationDiagram(Guid id, CancellationToken token)
    {
        var substation = await _mediator.Send(new GetSubstationDiagramQuery() { Id = id }, token);
        return Ok(substation);
    }

    // PUT api/Substations/5
    [HttpPut("{id}")]
    [Authorize(Roles = "Admins")]
    public async Task<IActionResult> PutSubstationDiagram(Guid id, [FromBody]DiagramModel diagram, CancellationToken token)
    {
        await _mediator.Send(new UpdateSubstationCommand() {Id = id, Diagram = diagram }, token);
        return NoContent();
    }

    // GET api/Substations/GetDiagramSVG/{id}
    [HttpGet("GetDiagramSVG/{id}")]
    public async Task<IActionResult> GetDiagramSVG(Guid id, CancellationToken token)
    {
        var substation = await _mediator.Send(new GetSubstationDiagramQuery() { Id = id }, token);
        return Content(substation.toSVG());
    }

    // GET api/Substations/GetSubstationName/{id}
    [HttpGet("GetSubstationName/{id}")]
    public async Task<ActionResult<string>> GetSubstationName(Guid id, CancellationToken token)
    {
        var substationName = await _mediator.Send(new GetSubstationNameQuery() { Id = id }, token);
        return substationName;
    }

    // GET api/Substations/GetSubstationEquipment/{id}
    [HttpGet("GetSubstationEquipment/{substationId}")]
    public async Task<ActionResult<IEnumerable<ValueLabel>>> GetSubstationEquipment(Guid substationId, CancellationToken token)
    {
        var equipments = await _mediator.Send(new GetSubstationEquipmentsQuery() { SubstationId = substationId }, token);
        return Ok(equipments);
    }
}
