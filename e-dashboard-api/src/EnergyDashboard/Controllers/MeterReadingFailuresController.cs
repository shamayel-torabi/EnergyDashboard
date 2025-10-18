using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using EnergyDashboard.Application.MeterReadingFailures;
using EnergyDashboard.Application.MeterReadingFailures.Queries;
using EnergyDashboard.Application.MeterReadingFailures.Commands;
using MediatR;

namespace EnergyDashboard.Controllers;

[Route("[controller]")]
[ApiController]

public class MeterReadingFailuresController : ControllerBase
{
    private readonly ISender _mediator;

    public MeterReadingFailuresController(ISender mediator)
    {
        _mediator = mediator;
    }

    // GET: api/MeterReadingFailures
    [HttpGet]
    public async Task<ActionResult<IEnumerable<MeterReadingFailureDto>>> GetMeterReadingFailures(CancellationToken token)
    {
        var meterReadingFailures = await _mediator.Send(new GetMeterReadingFailuresQuery(), token);
        return Ok(meterReadingFailures);
    }

    // GET: api/MeterReadingFailures/5
    [HttpGet("{id}")]
    public async Task<ActionResult<MeterReadingFailureDto>> GetMeterReadingFailure(Guid id, CancellationToken token)
    {
        var meterReadingFailure = await _mediator.Send(new GetMeterReadingFailureQuery() { Id = id}, token);
        return Ok(meterReadingFailure);
    }

    // PUT: api/MeterReadingFailures/5
    [HttpPut("{id}")]
    [Authorize(Roles = "Admins")]
    public async Task<IActionResult> PutMeterReadingFailure(Guid id, MeterReadingFailureDto meterReadingFailure, CancellationToken token)
    {
        if (id != meterReadingFailure.Id)
        {
            return BadRequest();
        }

        await _mediator.Send(new UpdateMeterReadingFailureCommand() { Id = id, MeterReadingFailure = meterReadingFailure }, token);

        return NoContent();
    }

    // POST: api/MeterReadingFailures
    [HttpPost]
    [Authorize(Roles = "Admins")]
    public async Task<ActionResult<MeterReadingFailureDto>> PostMeterReadingFailure(MeterReadingFailureDto meterReadingFailure, CancellationToken token)
    {
        await _mediator.Send(new CreateMeterReadingFailureCommand() { MeterReadingFailure = meterReadingFailure }, token);
        return CreatedAtAction("GetMeterReadingFailure", new { id = meterReadingFailure.Id }, meterReadingFailure);
    }

    // DELETE: api/MeterReadingFailures/5
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admins")]
    public async Task<ActionResult<MeterReadingFailureDto>> DeleteMeterReadingFailure(Guid id, CancellationToken token)
    {
        var meterReadingFailure = await _mediator.Send(new DeleteMeterReadingFailureCommand() { Id = id }, token);
        return Ok(meterReadingFailure);
    }

    // GET: api/MeterReadingFailures/GetMeterFailures
    [HttpGet("GetMeterFailures")]
    public async Task<ActionResult<IEnumerable<MeterReadingFailureDto>>> MeterFailures(CancellationToken token)
    {
        var meterReadingFailures = await _mediator.Send(new GetMeterFailuresQuery(), token);
        return Ok(meterReadingFailures);
    }
}
