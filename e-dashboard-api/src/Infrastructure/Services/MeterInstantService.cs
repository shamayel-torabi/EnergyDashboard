using System.Text;
using EnergyDashboard.Application.Interfaces;
using EnergyDashboard.Application.Models.Meters;
using EnergyDashboard.Domain.Entities;
using EnergyDashboard.Domain.ValueObjects;
using EnergyDashboard.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace EnergyDashboard.Infrastructure.Services;

public class MeterInstantService : IMeterInstantService
{
	private readonly AppDbContext _context;
	private readonly IDistributedCache _cache;
    private Dictionary<int, string> _powerplantOperatorNames = new();

	public MeterInstantService(
		AppDbContext context,
		IDistributedCache cache)
	{
		_context = context ?? throw new ArgumentNullException(nameof(context));
		_cache = cache ?? throw new ArgumentNullException(nameof(cache));
        GetPowerPlantOperatorNames();
    }

	public async Task<MeterInstant> GetMeterInstantAsync(string serialNumber, CancellationToken cancellationToken)
	{
		MeterInstant result = await GetMeterInstantsFromCache(serialNumber, cancellationToken);
		return result;
	}

	public async Task<IEnumerable<PowerPlantInstantPower>> GetPowerPlantInstantPowersAsync(CancellationToken cancellationToken)
	{
		string casheKey = "PowerPlantsInstantPower";
		byte[] encoded = await _cache.GetAsync(casheKey, cancellationToken);
		IEnumerable<PowerPlantInstantPower> powerPlantInstantPowers;
		if (encoded is not null)
		{
			string serialized = Encoding.UTF8.GetString(encoded);
			powerPlantInstantPowers = JsonConvert.DeserializeObject<IEnumerable<PowerPlantInstantPower>>(serialized);
		}
		else
		{
			powerPlantInstantPowers = await GetPowerPlantInstantPowers(cancellationToken);
			string serialized = JsonConvert.SerializeObject(powerPlantInstantPowers);
			encoded = Encoding.UTF8.GetBytes(serialized);
			DistributedCacheEntryOptions option = new DistributedCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(1.0)).SetAbsoluteExpiration(DateTime.Now.AddMinutes(5.0));
			await _cache.SetAsync(casheKey, encoded, option, cancellationToken);
		}
		return powerPlantInstantPowers;
	}

	public async Task<TotalInstantPower> GetTotalInstantPowersAsync(CancellationToken cancellationToken)
	{
		string casheKey = "TotalInstantPower";
		byte[] encoded = await _cache.GetAsync(casheKey, cancellationToken);
		TotalInstantPower totalInstantPower;
		if (encoded is not null)
		{
			string serialized = Encoding.UTF8.GetString(encoded);
			totalInstantPower = JsonConvert.DeserializeObject<TotalInstantPower>(serialized);
		}
		else
		{
			totalInstantPower = await GetTotalInstantPowers(cancellationToken);
			string serialized = JsonConvert.SerializeObject(totalInstantPower);
			encoded = Encoding.UTF8.GetBytes(serialized);
			DistributedCacheEntryOptions option = new DistributedCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(1.0)).SetAbsoluteExpiration(DateTime.Now.AddMinutes(5.0));
			await _cache.SetAsync(casheKey, encoded, option);
		}
		return totalInstantPower;
	}

	private async Task<IEnumerable<PowerPlantInstantPower>> GetPowerPlantInstantPowers(CancellationToken cancellationToken)
	{
		var powerPlants = await _context.GeneratorFeeders.ToListAsync(cancellationToken);

		var powerPlantsGroupe = powerPlants
            .GroupBy(g => g.PowerplantOperatorId)
            .Select(s => new
			{
				PowerPlantId = s.Key,
				Units = s.Select(x => new
				{
					Name = x.Name,
					Capacity = x.NominalCapacity,
					SerialNumber = x.GetMeterSerialNumber(),
				}).OrderBy(o => o.Name)

			}).OrderBy(oo => oo.PowerPlantId)
			.ToList();

        IList<PowerPlantInstantPower> powerPlantInstantPowers = new List<PowerPlantInstantPower>();

		foreach (var powerPlant in powerPlantsGroupe)
		{
			PowerPlantInstantPower item = new PowerPlantInstantPower
			{
				PowerPlantId = powerPlant.PowerPlantId,
				PowerPlantName = _powerplantOperatorNames[powerPlant.PowerPlantId],
            };

			foreach (var unit in powerPlant.Units)
			{
				UnitInstantPower ip = new UnitInstantPower
				{
					Name = unit.Name,
					Capacity = (unit.Capacity ?? 320.0)
				};
				UnitInstantPower unitInstantPower = ip;
				unitInstantPower.ActivePower = await GetMeterInstantPower(unit.SerialNumber, cancellationToken);
				item.UnitInstantPowers.Add(ip);
			}
			powerPlantInstantPowers.Add(item);
		}

		return powerPlantInstantPowers;
	}

	private async Task<TotalInstantPower> GetTotalInstantPowers(CancellationToken cancellationToken)
	{
		double genTotalInstantPower = await GetTotalPowerPlantInstantPowersAsync(cancellationToken);
		double induTotalInstantPower = await GetTotalIndustrialInstantPowersAsync(cancellationToken);
		double distTotalInstantPower = await GetTotalDistributionInstantPowersAsync(cancellationToken);
		double tabadolTotalInstantPower = await GetTotalTabadolInstantPowersAsync(cancellationToken);
		return new TotalInstantPower
		{
			Generation = genTotalInstantPower,
			Industrial = induTotalInstantPower,
			Distribution = distTotalInstantPower,
			Tabadol = tabadolTotalInstantPower
		};
	}

	private async Task<double> GetTotalPowerPlantInstantPowersAsync(CancellationToken cancellationToken)
	{
        var powerPlants = await _context.GeneratorFeeders.ToListAsync(cancellationToken);


        double sum = 0.0;
		foreach (var item in powerPlants)
		{
			EquipmentMeter meter = item.GetActiveMeter();
			if (meter is not null)
			{
				double num = sum;
				sum = num + await GetMeterInstantPower(meter.SerialNumber, cancellationToken);
			}
		}
		return sum;
	}

	private async Task<double> GetTotalIndustrialInstantPowersAsync(CancellationToken cancellationToken)
	{
		var loadFeeders = await _context.LoadFeeders.Where(w => w.LoadFeederTypeId == 2).ToListAsync(cancellationToken);

        double sum = 0.0;
		foreach (var item in loadFeeders)
		{
			EquipmentMeter meter = item.GetActiveMeter();
			if (meter is not null)
			{
				double num = await GetMeterInstantPower(meter.SerialNumber, cancellationToken);
				sum += num;
			}
		}
		return sum;
	}

	private async Task<double> GetTotalDistributionInstantPowersAsync(CancellationToken cancellationToken)
    {
        var loadFeeders = await _context.LoadFeeders.Where(w => w.LoadFeederTypeId == 1).ToListAsync(cancellationToken);

        double sum = 0.0;
		foreach (var item in loadFeeders)
		{
			EquipmentMeter meter = item.GetActiveMeter();
			if (meter is not null)
			{
                double num = await GetMeterInstantPower(meter.SerialNumber, cancellationToken);
                sum += num;
            }
        }
		return sum;
	}

	private async Task<double> GetTotalTabadolInstantPowersAsync(CancellationToken cancellationToken)
	{
        var lineFeeders = await _context.LineFeeders.ToListAsync(cancellationToken);

        double sum = 0.0;
		foreach (Equipment item in lineFeeders)
		{
			EquipmentMeter meter = item.GetActiveMeter();
			if (meter is not null)
			{
				double num = sum;
				sum = num + await GetMeterInstantPower(meter.SerialNumber, cancellationToken);
			}
		}
		return sum;
	}

	private async Task<double> GetMeterInstantPower(string serialNumber, CancellationToken cancellationToken)
	{
		double ActivePower = 0.0;

        MeterInstant mip = await GetMeterInstantsFromCache(serialNumber, cancellationToken);

        if (mip is not null)
        {
            decimal PowerActiveExport = mip.PExport;
            decimal PowerActiveImport = mip.PImport;
            ActivePower = Math.Abs((double)PowerActiveExport - (double)PowerActiveImport);
        }

        return ActivePower;
	}

    private async Task<MeterInstant> GetMeterInstantsFromCache(string serialNumber, CancellationToken cancellationToken)
    {
		MeterInstant meterInstant = null;
        byte[] encodedMeterInstantEnergy = await _cache.GetAsync(serialNumber, cancellationToken);
        if (encodedMeterInstantEnergy is not null)
        {
            string serializedMeterInstantEnergy = Encoding.UTF8.GetString(encodedMeterInstantEnergy);
            meterInstant = JsonConvert.DeserializeObject<MeterInstant>(serializedMeterInstantEnergy);
        }
        return meterInstant;
    }

    private void GetPowerPlantOperatorNames()
    {
        var powerplantOperators = _context.PowerplantOperators.Select(s => new { Id = s.Id, Name = s.Name }).ToList();
        foreach (var powerplantOperator in powerplantOperators)
            _powerplantOperatorNames.Add(powerplantOperator.Id, powerplantOperator.Name);
    }
}
