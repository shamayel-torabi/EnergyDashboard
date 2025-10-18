using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using EnergyDashboard.Application.Models.Meters;
using EnergyDashboard.Application.Meters;
using EnergyDashboard.Application.Meters.Querie;
using EnergyDashboard.Application.Meters.Commands;
using MediatR;

#nullable disable

namespace EnergyDashboard.Controllers;

[Route("[controller]")]
[ApiController]

public class MetersController : ControllerBase
{
    private readonly ISender _mediator;

    public MetersController(ISender mediator)
    {
        _mediator = mediator;
    }

    // GET: api/Meters
    [HttpGet]
    public async Task<ActionResult<IEnumerable<MeterDto>>> GetMeters(CancellationToken token)
    {
        var meters = await _mediator.Send(new GetMetersQuery() , token);
        return Ok(meters);
    }

    // GET: api/Meters/5
    [HttpGet("{serialNumber}")]
    public async Task<ActionResult<MeterDto>> GetMeter([FromRoute] string serialNumber, CancellationToken token)
    {
        var meter = await _mediator.Send(new GetMeterQuery() { SerialNumber = serialNumber }, token);
        return Ok(meter);
    }


    // PUT: api/Meters/5
    [HttpPut("{id}")]
    [Authorize(Roles = "Admins")]

    public async Task<IActionResult> PutMeters([FromRoute] int id,[FromBody] MeterDto meterDto, CancellationToken token)
    {
        if (id != meterDto.MeterId)
        {
            return BadRequest();
        }
        
        await _mediator.Send(new UpdateMeterCommand() { Id = id, Meter = meterDto }, token);
        return NoContent();
    }

    // DELETE: api/Meters/5
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admins")]
    public async Task<ActionResult<MeterDto>> DeleteMeter([FromRoute] int id, CancellationToken token)
    {
        await _mediator.Send(new DeleteMeterCommand() { Id = id}, token);
        return Ok();
    }

    // GET: api/Meters/UpdateMeterTabelss
    [HttpPut("UpdateMeterTabels")]
    [Authorize(Roles = "Admins")]
    public async Task<ActionResult> UpdateMeterTabels(CancellationToken token)
    {
        await _mediator.Send(new UpdateMeterTableCommand() , token);
        return NoContent();
    }

    // GET: api/Meters/GetActiveMeters
    [HttpGet("GetActiveMeters")]
    public async Task<ActionResult<IEnumerable<MeterEntity>>> GetActiveMeters(CancellationToken token)
    {
        var meters = await _mediator.Send(new GetActiveMetersQuery(), token);
        return Ok(meters);
    }

    // GET: api/Meters/GetSubstations
    [HttpGet("GetSubstations")]
    public async Task<ActionResult> GetSubstations(CancellationToken token)
    {
        var substations = await _mediator.Send(new GetSubstationsQuery(), token);
        return Ok(substations);
    }

    // GET: api/Meters/GetSubstationMeters/{substationId}
    [HttpGet("GetSubstationMeters/{substationId}")]
    public async Task<ActionResult<IEnumerable<MeterDto>>> GetSubstationMeters([FromRoute] int substationId, CancellationToken token)
    {
        var result = await _mediator.Send(new GetSubstationMetersQuery { SubstationId = substationId }, token);
        return Ok(result);
    }
}
