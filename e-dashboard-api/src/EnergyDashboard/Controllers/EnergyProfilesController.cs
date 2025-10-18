using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using EnergyDashboard.Domain.Entities;
using EnergyDashboard.Application.EnergyProfiles;
using EnergyDashboard.Application.EnergyProfiles.Queries;
using EnergyDashboard.Application.EnergyProfiles.Commands;
using MediatR;

namespace EnergyDashboard.Controllers;

[Route("[controller]")]
[ApiController]
public class EnergyProfilesController : ControllerBase
{
    private readonly ISender _mediator;

    public EnergyProfilesController(ISender mediator)
    {
        _mediator = mediator;
    }

    // GET: api/EnergyProfiles
    [HttpGet]
    public async Task<ActionResult<IEnumerable<EnergyProfileDto>>> GetEnergyProfiles(CancellationToken token)
    {
        var energyProfiles = await _mediator.Send(new GetEnergyProfilesQuery(), token);
        return Ok(energyProfiles);
    }

    // GET: api/EnergyProfiles/5
    [HttpGet("{id}")]
    public async Task<ActionResult<EnergyProfileDto>> GetEnergyProfile(Guid id, CancellationToken token)
    {
        var energyProfiles = await _mediator.Send(new GetEnergyProfileQuery() { Id = id }, token);
        return Ok(energyProfiles);

    }

    // PUT: api/EnergyProfiles/5
    [HttpPut("{id}")]
    [Authorize(Roles = "Admins")]
    public async Task<IActionResult> PutEnergyProfile(Guid id, EnergyProfileDto energyProfile, CancellationToken token)
    {
        if (id != energyProfile.Id)
        {
            return BadRequest();
        }

        await _mediator.Send(new UpdateEnergyProfileCommand() { Id = id, EnergyProfile = energyProfile }, token);
        return NoContent();
    }

    // POST: api/EnergyProfiles
    [HttpPost]
    [Authorize(Roles = "Admins")]
    public async Task<ActionResult<EnergyProfileDto>> PostEnergyProfile(EnergyProfileDto energyProfile, CancellationToken token)
    {
        await _mediator.Send(new CreateEnergyProfileCommand() { EnergyProfile = energyProfile }, token);
        return CreatedAtAction("GetEnergyProfile", new { id = energyProfile.Id }, energyProfile);
    }

    // DELETE: api/EnergyProfiles/5
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admins")]
    public async Task<ActionResult<EnergyProfile>> DeleteEnergyProfile(Guid id, CancellationToken token)
    {
        var energyProfile = await _mediator.Send(new DeleteEnergyProfileCommand() { Id = id }, token);
        return Ok(energyProfile);
    }

    #region  EquipmentEnergy
    // GET: api/EnergyProfiles/GetEquipmentEnergyProfile/year/month
    [HttpGet("GetEquipmentEnergyProfile/{equipmentId}/{year}/{month}")]
    public async Task<ActionResult<IEnumerable<EnergyProfileDto>>> GetEquipmentEnergyProfile([FromRoute] Guid equipmentId, [FromRoute] int year, [FromRoute] int month, CancellationToken token)
    {
        var energyProfile = await _mediator.Send(new GetEquipmentEnergyProfileQuery() { EquipmentId = equipmentId, Year = year, Month = month }, token);
        return Ok(energyProfile);
    }

    // GET: api/EnergyProfiles/GetEquipmentEnergyProfileHour/equipmentId/year/month
    [HttpGet("GetEquipmentEnergyProfileHour/{equipmentId}/{year}/{month}")]
    public async Task<ActionResult<IEnumerable<EquipmentEnergyProfile>>> GetEquipmentEnergyProfileHour([FromRoute] Guid equipmentId, [FromRoute] int year, [FromRoute] int month, CancellationToken token)
    {
        var energyProfile = await _mediator.Send(new GetEquipmentEnergyProfileHourQuery() { EquipmentId = equipmentId, Year = year, Month = month }, token);
        return Ok(energyProfile);
    }

    // GET: api/EnergyProfiles/GetUnitEnergyProfileHour/equipmentId/year/month
    [HttpGet("GetUnitEnergyProfileHour/{equipmentId}/{year}/{month}")]
    public async Task<ActionResult<IEnumerable<EquipmentEnergyProfile>>> GetUnitEnergyProfileHour([FromRoute] Guid equipmentId, [FromRoute] int year, [FromRoute] int month, CancellationToken token)
    {
        var energyProfile = await _mediator.Send(new GetUnitEnergyProfileHourQuery() { EquipmentId = equipmentId, Year = year, Month = month }, token);
        return Ok(energyProfile);
    }

    // GET: api/EnergyProfiles/GetEquipmentEnergyProfile/equipmentId/start/end
    [HttpGet("GetEquipmentEnergyProfileInterval/{equipmentId}/{start}/{end}")]
    public async Task<ActionResult<IEnumerable<EnergyProfileDto>>> GetEquipmentEnergyProfileInterval([FromRoute] Guid equipmentId, [FromRoute] DateTimeOffset start, [FromRoute] DateTimeOffset end, CancellationToken token)
    {
        var energyProfile = await _mediator.Send(new GetEquipmentEnergyProfileIntervalQuery() { EquipmentId = equipmentId, StartDate = start, EndDate = end }, token);
        return Ok(energyProfile);
    }

    // GET: api/EnergyProfiles/GetEquipmentDailyEnergy/{equipmentId}/{date}
    [HttpGet("GetEquipmentDailyEnergy/{equipmentId}/{date}")]
    public async Task<ActionResult<IEnumerable<EquipmentDailyEnergy>>> GetEquipmentDailyEnergy([FromRoute] Guid equipmentId, [FromRoute] DateTimeOffset date, CancellationToken token)
    {
        var energyProfile = await _mediator.Send(new GetEquipmentDailyEnergyQuery() { EquipmentId = equipmentId, RecordDate = date }, token);
        return Ok(energyProfile);
    }

    // GET: api/EnergyProfiles/GetSubstationEnergy/{substationId}/{date}
    [HttpGet("GetSubstationEnergy/{substationId}/{date}")]
    public async Task<ActionResult> GetSubstationEnergy([FromRoute] Guid substationId, [FromRoute] DateTimeOffset date, CancellationToken token)
    {
        var energyProfile = await _mediator.Send(new GetSubstationEnergyQuery() { SubstationId = substationId, RecordDate = date }, token);
        return Ok(energyProfile);
    }

    // GET: api/EnergyProfiles/GetPowerPlantEnergy/{powerplantId}/{date}
    [HttpGet("GetPowerPlantEnergy/{powerplantId}/{date}")]
    public async Task<ActionResult> GetPowerPlantEnergy([FromRoute] int powerplantId, [FromRoute] DateTimeOffset date, CancellationToken token)
    {
        var energyProfile = await _mediator.Send(new GetPowerPlantEnergyQuery() { PowerplantId = powerplantId, RecordDate = date }, token);
        return Ok(energyProfile);
    }

    #endregion

    #region UpdateEnergyProfile
    // GET: api/EnergyProfiles/UpdateEnergyProfile/{startDate}/{endDate}/{updateRecords}
    [HttpGet("UpdateEnergyProfile/{startDate}/{endDate}/{updateRecords}")]
    [Authorize(Roles = "Admins")]
    public async Task<ActionResult> UpdateEnergyProfile([FromRoute] DateTimeOffset startDate, [FromRoute] DateTimeOffset endDate, [FromRoute] bool updateRecords = false, CancellationToken token = default)
    {
        await _mediator.Send(new UpdateEquipmentsEnergyProfileCommand() { StartDate = startDate, EndDate = endDate, UpdateRecords = updateRecords }, token);
        var culture = new CultureInfo("fa-IR");
        return Ok(new { Result = $"پایان به روز رسانی جدول پروفایل و انرژی روزانه از تاریخ {startDate.ToString("dd MMMM yyyy", culture)} لغایت {endDate.ToString("dd MMMM yyyy", culture)}" });
    }

    // GET: api/EnergyProfiles/UpdateEquipmentEnergy/{equipmentId}/{startDate}/{endDate}/{updateRecords}
    [HttpGet("UpdateEquipmentEnergy/{equipmentId}/{startDate}/{endDate}/{updateRecords}")]
    [Authorize(Roles = "Admins")]
    public async Task<ActionResult> UpdateEquipmentEnergyProfile([FromRoute] Guid equipmentId, [FromRoute] DateTimeOffset startDate, [FromRoute] DateTimeOffset endDate, [FromRoute] bool updateRecords = false, CancellationToken token = default)
    {
        await _mediator.Send(new UpdateEquipmentEnergyProfileCommand() { EquipmentId = equipmentId, StartDate = startDate, EndDate = endDate, UpdateRecords = updateRecords }, token);
        var culture = new CultureInfo("fa-IR");
        return Ok(new { Result = $"پایان به روز رسانی جدول پروفایل و انرژی روزانه از تاریخ {startDate.ToString("dd MMMM yyyy", culture)} لغایت {endDate.ToString("dd MMMM yyyy", culture)}" });
    }

    // GET: api/EnergyProfiles/UpdateSubstationEquipmentsEnergy/{substationId}/{startDate}/{endDate}/{updateRecords}
    [HttpGet("UpdateSubstationEquipmentsEnergy/{substationId}/{startDate}/{endDate}/{updateRecords}")]
    [Authorize(Roles = "Admins")]
    public async Task<ActionResult> UpdateSubstationEquipmentsEnergy([FromRoute] Guid substationId, [FromRoute] DateTimeOffset startDate, [FromRoute] DateTimeOffset endDate, [FromRoute] bool updateRecords = false, CancellationToken token = default)
    {
        await _mediator.Send(new UpdateSubstationEquipmentsEnergyCommand() { SubstationId = substationId, StartDate = startDate, EndDate = endDate, UpdateRecords = updateRecords }, token);
        var culture = new CultureInfo("fa-IR");
        return Ok(new { Result = $"پایان به روز رسانی جدول پروفایل و انرژی روزانه ایستگاه از تاریخ {startDate.ToString("dd MMMM yyyy", culture)} لغایت {endDate.ToString("dd MMMM yyyy", culture)}" });
    }

    // GET: api/EnergyProfiles/UpdateEquipmentEnergyProfile/{equipmentId}/{date}
    [HttpGet("UpdateEquipmentEnergyProfile/{equipmentId}/{date}")]
    [Authorize(Roles = "Admins")]
    public async Task<ActionResult> UpdateEquipmentEnergyProfile([FromRoute] Guid equipmentId, [FromRoute] DateTimeOffset date, CancellationToken token)
    {
        await _mediator.Send(new UpdateEquipmentEnergyProfileNewCommand() { EquipmentId = equipmentId, RecordDate = date }, token);
        return Ok(new { Result = "UpdateEquipmentEnergyProfile" });
    }

    #endregion
}
