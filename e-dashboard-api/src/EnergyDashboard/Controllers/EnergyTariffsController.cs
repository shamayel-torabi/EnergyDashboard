using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using EnergyDashboard.Application.EnergyTariffs.Queries;
using EnergyDashboard.Application.EnergyTariffs.Commands;
using EnergyDashboard.Application.EnergyTariffs;
using MediatR;

namespace EnergyDashboard.Controllers;
[Route("[controller]")]
[ApiController]

public class EnergyTariffsController : ControllerBase
{
    private readonly ISender _mediator;

    public EnergyTariffsController(ISender mediator)
    {
        _mediator = mediator;
    }

    // GET: api/EnergyTariffs
    [HttpGet]
    public async Task<ActionResult<IEnumerable<EnergyTariffDto>>> GetEnergyTariffs(CancellationToken token)
    {
        var energyTariffs = await _mediator.Send(new GetEnergyTariffsQuery(), token);
        return Ok(energyTariffs);
    }

    // GET: api/EnergyTariffs/5
    [HttpGet("{id}")]
    public async Task<ActionResult<EnergyTariffDto>> GetEnergyTariff(Guid id, CancellationToken token)
    {
        var energyTariff = await _mediator.Send(new GetEnergyTariffQuery() { Id = id }, token);
        return Ok(energyTariff);
    }

    // PUT: api/EnergyTariffs/5
    [HttpPut("{id}")]
    [Authorize(Roles = "Admins")]
    public async Task<IActionResult> PutEnergyTariff(Guid id, EnergyTariffDto energyTariff, CancellationToken token)
    {
        if (id != energyTariff.Id)
        {
            return BadRequest();
        }

        var command = new UpdateEnergyTariffCommand() {
            Id = id,
            Name = energyTariff.Name,
            StartDate = energyTariff.StartDate,
            EndDate = energyTariff.EndDate,
            EnergyTariffRates= energyTariff.EnergyTariffRates.ToList(),
        };

        await _mediator.Send(command, token);
        return NoContent();
    }

    // POST: api/EnergyTariffs
    [HttpPost]
    [Authorize(Roles = "Admins")]
    public async Task<ActionResult<EnergyTariffDto>> PostEnergyTariff(EnergyTariffDto energyTariff, CancellationToken token)
    {
        var command = new CreateEnergyTariffCommand()
        {
            Id = energyTariff.Id,
            Name = energyTariff.Name,
            StartDate = energyTariff.StartDate,
            EndDate = energyTariff.EndDate,
            EnergyTariffRates = energyTariff.EnergyTariffRates.ToList(),
        };

        var etf = await _mediator.Send(command, token);
        return Ok(etf);
    }

    // DELETE: api/EnergyTariffs/5
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admins")]
    public async Task<ActionResult<EnergyTariffDto>> DeleteEnergyTariff(Guid id, CancellationToken token)
    {
        var e = await _mediator.Send(new DeleteEnergyTariffCommand() { Id = id }, token);
        return Ok(e);
    }
}
