
using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.Logging;

namespace MeterService.gRPC.Services;

#nullable disable
public sealed class MeterCleint : IMeterClient
{
    private readonly MeterService.MeterServiceClient _client;
    private readonly ILogger<MeterCleint> _logger;

    public MeterCleint(MeterService.MeterServiceClient client, ILogger<MeterCleint> logger)
    {
        _client = client;
        _logger = logger;
    }

    public async Task<IEnumerable<Meter>> GetMeters()
    {
        var meters = new List<Meter>();

        try
        {
            var req = new Empty();
            var res = await _client.GetMetersAsync(req);

            if (res is not null)
            {
                foreach (var meter in res.Meters)
                {
                    meters.Add(meter);
                }
            }
        }
        catch (Exception)
        {
            _logger.LogWarning("Error GetMeters");
        }

        return meters;
    }

    public async Task<Meter> GetMeter(string serialNumber)
    {
        try
        {
            var req = new MeterRequest() { SerialNumber = serialNumber };
            var res = await _client.GetMeterAsync(req);
            return res;
        }
        catch (Exception)
        {
            _logger.LogWarning("Error GetMeter");
        }

        return null;
    }

    public async Task DeleteMeter(int id)
    {
        try
        {
            var req = new DeleteMeterRequest() { MeterId = id };
            var res = await _client.DeleteMeterAsync(req);
        }
        catch (Exception)
        {
            _logger.LogWarning("Error DeleteMeter");
        }
    }

    public async Task UpdateMeter(Meter meter)
    {
        try
        {
            var req = new UpdateMeterRequest() { Meter = meter };
            var res = await _client.UpdateMeterAsync(req);
        }
        catch (Exception)
        {
            _logger.LogWarning("Error UpdateMeter");
        }
    }

    public async Task<IEnumerable<Substation>> GetSubstations()
    {
        var substation = new List<Substation>();

        try
        {
            var req = new Empty();
            var res = await _client.GetSubstationsAsync(req);

            if (res is not null)
            {
                foreach (var sub in res.Substations)
                {
                    substation.Add(sub);
                }
            }
        }
        catch (Exception)
        {
            _logger.LogWarning("Error GetSubstations");
        }

        return substation;
    }

    public async Task<IEnumerable<Meter>> GetSubstationMeters(int substationId)
    {
        var meters = new List<Meter>();

        try
        {
            var req = new SubstationMetersRequest() { SubstationId = substationId };
            var res = await _client.GetSubstationMetersAsync(req);

            if (res is not null)
            {
                foreach (var meter in res.Meters)
                {
                    meters.Add(meter);
                }
            }
        }
        catch (Exception)
        {
            _logger.LogWarning("Error GetSubstationMeters");
        }

        return meters;
    }

    public async Task<bool> UpdateMeterTable()
    {
        try
        {
            var req = new Empty();
            var res = await _client.UpdateMeterTableAsync(req);
            return res.Value;

        }
        catch (Exception)
        {
            _logger.LogWarning("Error UpdateMeterTable");
        }

        return false;
    }

    public async Task<IEnumerable<MeterEnergy>> GetMetersEnergy(DateTimeOffset date)
    {
        var meterEnergys = new List<MeterEnergy>();

        try
        {
            var req = new MetersEnergyRequest() { Date = date.ToTimestamp()};
            var res = await _client.GetMetersEnergyAsync(req);

            if (res is not null)
            {
                foreach (var me in res.MeterEnergies)
                {
                    meterEnergys.Add(me);
                }
            }
        }
        catch (Exception)
        {
            _logger.LogWarning("Error GetMetersEnergy");
        }

        return meterEnergys;
    }

    public async Task<IEnumerable<MeterEnergy>> GetMeterEnergy(DateTimeOffset date, string serialNumber)
    {
        var meterEnergys = new List<MeterEnergy>();

        try
        {
            var req = new MeterEnergyRequest() { Date = date.ToTimestamp(), SerialNumber = serialNumber };
            var res = await _client.GetMeterEnergyAsync(req);

            if (res is not null)
            {
                foreach (var me in res.MeterEnergies)
                {
                    meterEnergys.Add(me);
                }
            }
        }
        catch (Exception)
        {
            _logger.LogWarning("Error GetMeterEnergy");
        }

        return meterEnergys;
    }

    public async Task DownloadMetersEnergy(DateTimeOffset startDate, DateTimeOffset endDate, bool update = false)
    {
        try
        {
            var req = new DownloadMetersEnergyRequest() { 
                StartDate = startDate.ToTimestamp(),
                EndDate = endDate.ToTimestamp(),
                Update = update,
            };
            await _client.DownloadMetersEnergyAsync(req);
        }
        catch (Exception)
        {
            _logger.LogWarning("Error DownloadMetersEnergy");
        }
    }

    public async Task DownloadMeterEnergy(DateTimeOffset startDate, DateTimeOffset endDate, string serialNumber, bool update = false)
    {
        try
        {
            var req = new DownloadMeterEnergyRequest()
            {
                StartDate = startDate.ToTimestamp(),
                EndDate = endDate.ToTimestamp(),
                Update = update,
                SerialNumber = serialNumber
            };
            await _client.DownloadMeterEnergyAsync(req);
        }
        catch (Exception)
        {
            _logger.LogWarning("Error DownloadMeterEnergy");
        }
    }
}
