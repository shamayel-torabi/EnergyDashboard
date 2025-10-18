using Microsoft.AspNetCore.Mvc;
using EnergyDashboard.Application.Utility;
using EnergyDashboard.Application.Models.Meters;
using EnergyDashboard.Application.Utility.Queries;
using MediatR;
using EnergyDashboard.Application.Models;

namespace EnergyDashboard.Controllers;

[Route("[controller]")]
[ApiController]
public class UtilityController : ControllerBase
{
    private readonly ISender _mediator;

    public UtilityController(ISender mediator)
    {
        _mediator = mediator;
    }

    // GET: api/Utility/GetPowerplantTypes
    [HttpGet("GetPowerplantTypes")]
    public async Task<ActionResult<IEnumerable<ValueLabel>>> GetPowerplantTypes(CancellationToken token)
    {
        var powerplantTypes = await _mediator.Send(new GetPowerplantTypesQuery(), token);
        return Ok(powerplantTypes);
    }

    // GET: api/Utility/GetPowerplantOperators
    [HttpGet("GetPowerplantOperators")]
    public async Task<ActionResult<IEnumerable<ValueLabel>>> GetPowerplantOperators(CancellationToken token)
    {
        var powerplantOperators = await _mediator.Send(new GetGetPowerplantOperatorsQuery(), token);
        return Ok(powerplantOperators);
    }

    // GET: api/Utility/GetPowerplantUnits/powerPlantId
    [HttpGet("GetPowerplantUnits/{powerPlantId}")]
    public async Task<ActionResult<IEnumerable<ValueLabel>>> GetPowerplantUnits(int powerPlantId, CancellationToken token)
    {
        var powerPlantUnits = await _mediator.Send(new GetPowerplantUnitsQuery() { PowerPlantId = powerPlantId }, token);
        return Ok(powerPlantUnits);
    }

    // GET: api/Utility/GetLoadFeederTypes
    [HttpGet("GetLoadFeederTypes")]
    public async Task<ActionResult<IEnumerable<LoadFeederTypesDto>>> GetLoadFeederTypes(CancellationToken token)
    {
        var loadFeederTypes = await _mediator.Send(new GetLoadFeederTypesQuery(), token);
        return Ok(loadFeederTypes);
    }

    // GET: api/Utility/GetSubstations
    [HttpGet("GetSubstations")]
    public async Task<ActionResult<IEnumerable<ValueLabel>>> GetSubstations(CancellationToken token)
    {
        var substations = await _mediator.Send(new GetSubstationsQuery(), token);
        return Ok(substations);
    }

    // GET: api/Utility/GetIGMCSubstations
    [HttpGet("GetIGMCSubstations")]
    public async Task<ActionResult<IEnumerable<ValueLabel>>> GetIGMCSubstations(CancellationToken token)
    {
        var substations = await _mediator.Send(new GetIGMCSubstationsQuery(), token);
        return Ok(substations);
    }


    // GET: api/Utility/GetSubstationMeters/substationId
    [HttpGet("GetSubstationMeters/{substationId}")]
    public async Task<ActionResult<IEnumerable<MeterEntity>>> GetSubstationMeters([FromRoute] int substationId, CancellationToken token)
    {
        var substationMeters = await _mediator.Send(new GetSubstationMetersQuery() { SubstationId = substationId }, token);
        return Ok(substationMeters);
    }
}
