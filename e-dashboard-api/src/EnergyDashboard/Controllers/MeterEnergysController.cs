using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using EnergyDashboard.Application.Models.Meters;
using EnergyDashboard.Application.MeterEnergys.Queries;
using EnergyDashboard.Application.MeterEnergys.Commands;
using MediatR;

#nullable disable

namespace EnergyDashboard.Controllers;

[Route("[controller]")]
[ApiController]
public class MeterEnergysController : ControllerBase
{
    private readonly ISender _mediator;

    public MeterEnergysController(ISender mediator)
    {
        _mediator = mediator;
    }


    // GET: api/MeterEnergy
    //[HttpGet]
    //public async Task<ActionResult<IEnumerable<MeterEnergyDto>>> GetMeterEnergys(CancellationToken token)
    //{
    //    var meterEnergys = await _mediator.Send(new GetMeterEnergysQuery() , token);
    //    return Ok(meterEnergys);
    //}

    //// GET: api/MeterEnergy/5
    //[HttpGet("{id}")]
    //public async Task<ActionResult<MeterEnergy>> GetMeterEnergy(Guid id, CancellationToken token)
    //{
    //    var meterEnergy = await _mediator.Send(new GetMeterEnergyQuery() { Id = id}, token);
    //    return Ok(meterEnergy);
    //}

    //// PUT: api/MeterEnergy/5
    //[HttpPut("{id}")]
    //[Authorize(Roles = "Admins")]
    //public async Task<IActionResult> PutMeterEnergy(Guid id, MeterEnergy meterEnergy)
    //{
    //    if (id != meterEnergy.MeterEnergyId)
    //    {
    //        return BadRequest();
    //    }

    //    _context.MeterEnergys.Update(meterEnergy);
    //    //_context.Entry(meterEnergy).State = EntityState.Modified;

    //    try
    //    {
    //        await _context.SaveChangesAsync();
    //    }
    //    catch (DbUpdateConcurrencyException)
    //    {
    //        if (!MeterEnergyExists(id))
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

    // POST: api/MeterEnergy
    //[HttpPost]
    //[Authorize(Roles = "Admins")]
    //public async Task<ActionResult<MeterEnergy>> PostMeterEnergy(MeterEnergy meterEnergy)
    //{
    //    _context.MeterEnergys.Add(meterEnergy);
    //    await _context.SaveChangesAsync();

    //    return CreatedAtAction("GetMeterEnergy", new { id = meterEnergy.MeterEnergyId }, meterEnergy);
    //}

    //// DELETE: api/MeterEnergy/5
    //[HttpDelete("{id}")]
    //[Authorize(Roles = "Admins")]
    //public async Task<ActionResult<MeterEnergy>> DeleteMeterEnergy(Guid id)
    //{
    //    var meterEnergy = await _context.MeterEnergys.FindAsync(id);
    //    if (meterEnergy == null)
    //    {
    //        return NotFound();
    //    }

    //    _context.MeterEnergys.Remove(meterEnergy);
    //    await _context.SaveChangesAsync();

    //    return meterEnergy;
    //}


    //// DELETE: api/MeterEnergy/DeleteAllMeterEnergy
    //[HttpGet("DeleteAllMeterEnergy")]
    //[Authorize(Roles = "Admins")]

    //public async Task<ActionResult> DeleteAllMeterEnergy()
    //{
    //    var meterEnergy = await _context.MeterEnergys.ToListAsync();
    //    if (meterEnergy == null)
    //    {
    //        return NotFound();
    //    }

    //    _context.MeterEnergys.RemoveRange(meterEnergy);
    //    await _context.SaveChangesAsync();

    //    return NoContent();
    //}


    //DELETE: api/MeterEnergy/DeleteAllMeterEnergyByDate/2021-02-19T19:30:00.000Z
    //[HttpGet("DeleteAllMeterEnergyByDate/{date}")]
    //[Authorize(Roles = "Admins")]
    //public async Task<ActionResult> DeleteAllMeterEnergyByDate([FromRoute] DateTimeOffset date, CancellationToken token)
    //{
    //    await _mediator.Send(new DeleteAllMeterEnergyByDateQuery() { RecordDate = date }, token);
    //    return Ok(new { Result = "Delete Records OK" });
    //}


    // GET: api/MeterEnergy/GetMeterEnergyByMeterId/1547
    //[HttpGet("GetMeterEnergyByMeterId/{meterId}")]
    //public async Task<ActionResult> GetMeterEnergyByMeterId([FromRoute] int meterId, CancellationToken token)
    //{
    //    var meterEnergys = await _mediator.Send(new GetMeterEnergyByMeterIdQuery() { MeterId = meterId }, token);

    //    if (meterEnergys == null)
    //    {
    //        return NotFound();
    //    }

    //    StringBuilder sb = new StringBuilder();
    //    sb.AppendLine("datetime,energy");
    //    foreach (var m in meterEnergys)
    //        sb.AppendLine($"{m.RecordDate.ToString("yyyy-MM-ddTHH:mm:ssK", CultureInfo.InvariantCulture)},{m.EnergyActiveExport - m.EnergyActiveImport}");

    //    MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(sb.ToString()));
    //    string fileName = $"{meterId}.csv";
    //    return File(ms, "text/csv", fileName);
    //}

    // api/MeterEnergy/GetMeterEnergyCount/2020-03-20T19:30:00.000Z/2021-02-19T20:30:00.000Z
    // api/MeterEnergy/GetMeterEnergyCount/{startDate}/{endDate}
    //[HttpGet("GetMeterEnergyCount/{startDate}/{endDate}")]
    //[Authorize(Roles = "Admins")]
    //public async Task<ActionResult> GetMeterEnergyCount([FromRoute] DateTimeOffset startDate, [FromRoute] DateTimeOffset endDate, CancellationToken token = default)
    //{
    //    await _mediator.Send(new GetMeterEnergyCountQuery() { StartDate = startDate, EndDate = endDate }, token);
    //    return Ok();
    //}

    [HttpGet("GetSubstationEnergy/{stationId}/{date}")]
    public async Task<ActionResult<IEnumerable<SubstationMeterEnergyList>>> GetSubstationEnergy([FromRoute] int stationId, [FromRoute] DateTimeOffset date, CancellationToken token = default)
    {
        var query = new GetSubstationEnergyQuery()
        {
            SubstationId = stationId,
            RecordDate = date
        };
        var substationsEnergy = await _mediator.Send(query, token);
        return Ok(substationsEnergy);
    }

    // GET: api/MeterEnergys/GetPaginatedMeterEnergyByDate/2022-04-18T19:30:00.000Z?pageIndex=0&pageSize=10
    [HttpGet("GetPaginatedMeterEnergyByDate/{date}")]
    public async Task<ActionResult> GetPaginatedMeterEnergyByDate([FromRoute] DateTimeOffset date, [FromQuery] int pageIndex, [FromQuery] int pageSize ,[FromQuery] bool abnormal,  CancellationToken token)
    {
        var query = new GetPaginatedMeterEnergyByDateQuery()
        {
            RecordDate = date,
            PageIndex = pageIndex,
            PageSize = pageSize,
            Abnormal = abnormal
        };
        var meterEnergyList = await _mediator.Send(query, token);
        return Ok(meterEnergyList);
    }

    // GET: api/MeterEnergys/GetPaginatedTransformersMeterEnergyByDate/2022-04-18T19:30:00.000Z?pageIndex=0&pageSize=10
    [HttpGet("GetPaginatedTransformersMeterEnergyByDate/{date}")]
    public async Task<ActionResult> GetPaginatedTransformersMeterEnergyByDate([FromRoute] DateTimeOffset date, [FromQuery] int pageIndex, [FromQuery] int pageSize, [FromQuery] bool abnormal, CancellationToken token)
    {
        var query = new GetPaginatedTransformersMeterEnergyByDateQuery()
        {
            RecordDate = date,
            PageIndex = pageIndex,
            PageSize = pageSize,
            Abnormal = abnormal            
        };
        var meterEnergyList = await _mediator.Send(query, token);
        return Ok(meterEnergyList);
    }

    // Post: api/MeterEnergy/UpdateMetersEnergy
    [HttpPost("UpdateMetersEnergy")]
    [Authorize(Roles = "Admins")]
    public async Task<ActionResult> UpdateMetersEnergy(DownloadMetersEnergyCommand command, CancellationToken token = default)
    {
        await _mediator.Send(command, token);
        return Ok(new { Result = $"به روز رسانی جدول انرژی میتر از تاریخ {command.StartDate.ToString("dd MMMM yyyy")} لغایت {command.EndDate.ToString("dd MMMM yyyy")} به صف دانلود افزوده شد." });
    }

    // POST: api/MeterEnergy/UpdateMeterEnergy
    [HttpPost("UpdateMeterEnergy")]
    [Authorize(Roles = "Admins")]
    public async Task<ActionResult> UpdateMeterEnergy(DownloadMeterEnergyCommand command, CancellationToken token = default)
    {
        await _mediator.Send(command, token);
        return Ok(new { Result = $"به روز رسانی جدول انرژی میتر از تاریخ {command.StartDate.ToString("dd MMMM yyyy")} لغایت {command.EndDate.ToString("dd MMMM yyyy")} به صف دانلود افزوده شد." });
    }


    // POST: api/MeterEnergy/UpdateEquipmentMeterEnergy
    [HttpPost("UpdateEquipmentMeterEnergy")]
    [Authorize(Roles = "Admins")]
    public async Task<ActionResult> UpdateEquipmentMeterEnergy(DownloadEquipmentMeterEnergyCommand command, CancellationToken token = default)
    {
        await _mediator.Send(command, token);
        return Ok(new { Result = $"به روز رسانی جدول انرژی میتر از تاریخ {command.StartDate.ToString("dd MMMM yyyy")} لغایت {command.EndDate.ToString("dd MMMM yyyy")}" });
    }
}
