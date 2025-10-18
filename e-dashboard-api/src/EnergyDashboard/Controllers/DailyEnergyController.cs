using Microsoft.AspNetCore.Mvc;
using EnergyDashboard.Application.DailyEnergys;
using EnergyDashboard.Application.DailyEnergys.Queries;
using MediatR;

namespace EnergyDashboard.Controllers;

[Route("[controller]")]
[ApiController]
public class DailyEnergyController : ControllerBase
{
    private readonly ISender _mediator;

    public DailyEnergyController(ISender mediator)
    {
        _mediator = mediator;
    }

    // GET: api/DailyEnergy
    [HttpGet]
    public async Task<ActionResult<IEnumerable<DailyEnergyDto>>> GetDailyEnergys(CancellationToken token)
    {
        var dailyEnergys = await _mediator.Send(new GetDailyEnergysQuery() , token);
        return Ok(dailyEnergys);
    }

    // GET: api/DailyEnergy/5
    [HttpGet("{id}")]
    public async Task<ActionResult<DailyEnergyDto>> GetDailyEnergy(Guid id, CancellationToken token)
    {
        var dailyEnergy = await _mediator.Send(new GetDailyEnergyQuery() {Id = id }, token);
        return Ok(dailyEnergy);
    }

    //// PUT: api/DailyEnergy/5
    //[HttpPut("{id}")]
    //[Authorize(Roles = "Admins")]

    //public async Task<IActionResult> PutDailyEnergy(Guid id, DailyEnergy dailyEnergy)
    //{
    //    if (id != dailyEnergy.DailyEnergyId)
    //    {
    //        return BadRequest();
    //    }


    //    _context.DailyEnergys.Update(dailyEnergy);
    //    //_context.Entry(dailyEnergy).State = EntityState.Modified;

    //    try
    //    {
    //        await _context.SaveChangesAsync();
    //    }
    //    catch (DbUpdateConcurrencyException)
    //    {
    //        if (!DailyEnergyExists(id))
    //        {
    //            return NotFound();
    //        }
    //        else
    //        {
    //            throw;
    //        }
    //    }

    //    return NoContent();
    //}

    //// POST: api/DailyEnergy
    //[HttpPost]
    //[Authorize(Roles = "Admins")]

    //public async Task<ActionResult<DailyEnergy>> PostDailyEnergy(DailyEnergy dailyEnergy)
    //{
    //    _context.DailyEnergys.Add(dailyEnergy);
    //    await _context.SaveChangesAsync();

    //    return CreatedAtAction("GetDailyEnergy", new { id = dailyEnergy.DailyEnergyId }, dailyEnergy);
    //}

    //// DELETE: api/DailyEnergy/5
    //[HttpDelete("{id}")]
    //[Authorize(Roles = "Admins")]

    //public async Task<ActionResult<DailyEnergy>> DeleteDailyEnergy(Guid id)
    //{
    //    var dailyEnergy = await _context.DailyEnergys.FindAsync(id);
    //    if (dailyEnergy == null)
    //    {
    //        return NotFound();
    //    }

    //    _context.DailyEnergys.Remove(dailyEnergy);
    //    await _context.SaveChangesAsync();

    //    return dailyEnergy;
    //}


    // GET: api/DailyEnergy/GetPowerPlantsEnergyYear/{year}

    [HttpGet("GetPowerPlantsEnergyYear/{year}")]
    public async Task<ActionResult<IOrderedEnumerable<YearlyPowerPlantEnergy>>> GetPowerPlantsEnergyYear([FromRoute] int year, CancellationToken token)
    {
        var yearlyEnergy = await _mediator.Send(new GetPowerPlantsEnergyYearQuery() { Year = year }, token);
        return Ok(yearlyEnergy);
    }

    // GET: api/DailyEnergy/GetUnitEnergyYear/{year}
    [HttpGet("GetUnitEnergyYear/{year}")]
    public async Task<ActionResult<IOrderedEnumerable<YearlyUnitEnergy>>> GetUnitEnergyYear([FromRoute] int year, CancellationToken token)
    {
        var yearlyEnergy = await _mediator.Send(new GetYearlyUnitEnergyQuery() { Year = year }, token);
        return Ok(yearlyEnergy);
    }

    // GET: api/DailyEnergy/GetIndustrialLoadEnergyYear/{year}
    [HttpGet("GetIndustrialLoadEnergyYear/{year}")]
    public async Task<ActionResult<IOrderedEnumerable<YearlyLoadEnergy>>> GetIndustrialLoadEnergyYear([FromRoute] int year, CancellationToken token)
    {
        var yearlyEnergy = await _mediator.Send(new GetYearlyIndustrialLoadEnergyQuery() { Year = year }, token);
        return Ok(yearlyEnergy);
    }

    // GET: api/DailyEnergy/GetDistributionEnergyYear/{year}
    [HttpGet("GetDistributionEnergyYear/{year}")]
    public async Task<ActionResult<IOrderedEnumerable<YearlyLoadEnergy>>> GetDistributionEnergyYear([FromRoute] int year, CancellationToken token)
    {
        var yearlyEnergy = await _mediator.Send(new GetYearlyDistributionEnergyQuery() { Year = year }, token);
        return Ok(yearlyEnergy);
    }


    // GET: api/DailyEnergy/GetPowerPlantsMonthly/{year}
    [HttpGet("GetPowerPlantsMonthly/{year}")]
    public async Task<ActionResult<IOrderedEnumerable<MonthlyPowerPlantsEnergy>>> GetPowerPlantsMonth([FromRoute] int year, CancellationToken token)
    {
        var mothlyEnergy = await _mediator.Send(new GetMonthlyPowerPlantsEnergyQuery() { Year = year }, token);
        return Ok(mothlyEnergy);
    }

    // GET: api/DailyEnergy/GetUnitMonthly/{year}
    [HttpGet("GetUnitMonthly/{year}")]
    public async Task<ActionResult<IOrderedEnumerable<MonthlyPowerPlantUnitEnergy>>> GetUnitMonthly([FromRoute] int year, CancellationToken token)
    {
        var mothlyEnergy = await _mediator.Send(new GetMonthlyUnitEnergyEnergyQuery() { Year = year }, token);
        return Ok(mothlyEnergy);
    }

    // GET: api/DailyEnergy/GetIndustrialLoadMonthly/{year}
    [HttpGet("GetIndustrialLoadMonthly/{year}")]
    public async Task<ActionResult> GetIndustrialLoadMonthly([FromRoute] int year, CancellationToken token)
    {
        var mothlyEnergy = await _mediator.Send(new GetMonthlyIndustrialEnergyQuery() { Year = year }, token);
        return Ok(mothlyEnergy);
    }

    // GET: api/DailyEnergy/GetDistributionMonthly/{year}
    [HttpGet("GetDistributionMonthly/{year}")]
    public async Task<ActionResult> GetDistributionMonthly([FromRoute] int year, CancellationToken token)
    {
        var mothlyEnergy = await _mediator.Send(new GetMonthlyDistributionEnergyQuery() { Year = year }, token);
        return Ok(mothlyEnergy);
    }



    // GET: api/DailyEnergy/GetPowerPlantsEnergyDaily/{year}/{month}
    [HttpGet("GetPowerPlantsEnergyDaily/{year}/{month}")]
    public async Task<ActionResult<IOrderedEnumerable<DayPowerPlantsEnergy>>> GetPowerPlantsEnergyDaily([FromRoute] int year, [FromRoute] int month, CancellationToken token)
    {
        var mothlyEnergy = await _mediator.Send(new GetDailyPowerPlantsEnergyQuery() { Year = year, Month = month }, token);
        return Ok(mothlyEnergy);
    }

    // GET: api/DailyEnergy/GetUnitEnergyDaily/{year}/{month}
    [HttpGet("GetUnitEnergyDaily/{year}/{month}")]
    public async Task<ActionResult<IOrderedEnumerable<DayPowerPlantUnitsEnergy>>> GetUnitEnergyDaily([FromRoute] int year, [FromRoute] int month, CancellationToken token)
    {
        var mothlyEnergy = await _mediator.Send(new GetDailyUnitEnergyQuery() { Year = year, Month = month }, token);
        return Ok(mothlyEnergy);
    }

    // GET: api/DailyEnergy/GetIndustrialLoadEnergyDaily/{year}/{month}
    [HttpGet("GetIndustrialLoadEnergyDaily/{year}/{month}")]
    public async Task<ActionResult<IOrderedEnumerable<DayLoadEnergy>>> GetIndustrialLoadEnergyDaily([FromRoute] int year, [FromRoute] int month, CancellationToken token)
    {
        var mothlyEnergy = await _mediator.Send(new GetDailyIndustrialEnergyQuery() { Year = year, Month = month }, token);
        return Ok(mothlyEnergy);
    }

    // GET: api/DailyEnergy/GetDistributionEnergyDaily/{year}/{month}
    [HttpGet("GetDistributionEnergyDaily/{year}/{month}")]
    public async Task<ActionResult<IOrderedEnumerable<DayLoadEnergy>>> GetDistributionEnergyDaily([FromRoute] int year, [FromRoute] int month, CancellationToken token)
    {
        var mothlyEnergy = await _mediator.Send(new GetDailyDistributionEnergyQuery() { Year = year, Month = month }, token);
        return Ok(mothlyEnergy);
    }



    // GET: api/DailyEnergy/GetPowerPlantsEnergyMonth/{year}/{month}
    [HttpGet("GetPowerPlantsEnergyMonth/{year}/{month}")]
    public async Task<ActionResult<IOrderedEnumerable<YearlyPowerPlantEnergy>>> GetPowerPlantsEnergyMonth([FromRoute] int year, [FromRoute] int month, CancellationToken token)
    {
        var mothlyEnergy = await _mediator.Send(new GetPowerPlantsEnergyMonthQuery() { Year = year, Month = month }, token);
        return Ok(mothlyEnergy);
    }

    // GET: api/DailyEnergy/GetPowerPlantsEnergy/{date}
    [HttpGet("GetPowerPlantsEnergy/{date}")]
    public async Task<ActionResult<IOrderedEnumerable<YearlyPowerPlantEnergy>>> GetPowerPlantsEnergy([FromRoute] DateTime date, CancellationToken token)
    {
        var mothlyEnergy = await _mediator.Send(new GetPowerPlantsEnergyQuery() { RecordDate = date }, token);
        return Ok(mothlyEnergy);
    }

    // GET: api/DailyEnergy/GetSubstationEnergy/{substationId}/{date}
    [HttpGet("GetSubstationEnergy/{substationId}/{date}")]
    public async Task<ActionResult<IOrderedEnumerable<SubstationEnergy>>> GetSubstationEnergy([FromRoute] Guid substationId, [FromRoute] DateTime date, CancellationToken token)
    {
        var mothlyEnergy = await _mediator.Send(new GetSubstationEnergyQuery() { SubstationId = substationId,  RecordDate = date }, token);
        return Ok(mothlyEnergy);
    }


    // GET: api/DailyEnergy/EnergyBalance/date
    [HttpGet("EnergyBalance/{date}")]
    public async Task<ActionResult<IOrderedEnumerable<EnergyBalance>>> EnergyBalance([FromRoute] DateTime date, CancellationToken token)
    {
        var mothlyEnergy = await _mediator.Send(new GetEnergyBalanceQuery() { RecordDate = date }, token);
        return Ok(mothlyEnergy);
    }

    // GET: api/DailyEnergy/SubstationEnergyBalance/EFF100B1-D960-4C96-94F0-1F21A331A03B/2020-03-21T19:30:00.000Z
    [HttpGet("SubstationEnergyBalance/{substationId}/{date}")]
    public async Task<ActionResult<IOrderedEnumerable<SubstationEnergyBalance>>> SubstationEnergyBalance([FromRoute] Guid substationId, [FromRoute] DateTime date, CancellationToken token)
    {
        var mothlyEnergy = await _mediator.Send(new GetSubstationEnergyBalanceQuery() { SubstationId = substationId,  RecordDate = date }, token);
        return Ok(mothlyEnergy);
    }
}
