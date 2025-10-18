using EnergyDashboard.Application.Models.Meters;
using EnergyDashboard.Domain.ValueObjects;

namespace EnergyDashboard.Application.Interfaces;

public interface IMeterInstantService
{
	//Task<IEnumerable<MeterInstant>> GetMeterInstantsAsync(CancellationToken cancellationToken);

	Task<MeterInstant> GetMeterInstantAsync(string serialNumber, CancellationToken cancellationToken);

	Task<IEnumerable<PowerPlantInstantPower>> GetPowerPlantInstantPowersAsync(CancellationToken cancellationToken);

	Task<TotalInstantPower> GetTotalInstantPowersAsync(CancellationToken cancellationToken);
}
