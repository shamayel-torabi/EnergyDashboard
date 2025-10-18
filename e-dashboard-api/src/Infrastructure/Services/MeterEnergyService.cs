using System.Text;
using EnergyDashboard.Application.Interfaces;
using EnergyDashboard.Application.MeterEnergys;
using EnergyDashboard.Application.Meters;
using EnergyDashboard.Application.Models.Meters;
using MeterService.gRPC.Services;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace EnergyDashboard.Infrastructure.Services;

public class MeterEnergyService : IMeterEnergyService
{
    private readonly IMeterClient _meterServiceClient;
    private readonly IDistributedCache _cache;

    public MeterEnergyService(IMeterClient meterServiceClient, IDistributedCache cache)
    {
        _meterServiceClient = meterServiceClient ?? throw new ArgumentNullException(nameof(meterServiceClient));
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
    }

    public async Task<List<MeterEnergyList>> GetMeterEnergyFromCacheByDate(DateTime date, CancellationToken cancellationToken)
    {
        var meters = await GetMetersFromCash(cancellationToken);

        string casheKey = $"MeterEnergy-{date.Date.Ticks}";

        List<MeterEnergyList> meterEnergyList = null;
        List<MeterEnergyDto> meterEnergys = new List<MeterEnergyDto>();

        string serializedMeterEnergy;
        var encodedMeterEnergy = await _cache.GetAsync(casheKey, cancellationToken);

        if (encodedMeterEnergy is not null)
        {
            serializedMeterEnergy = Encoding.UTF8.GetString(encodedMeterEnergy);
            meterEnergyList = JsonConvert.DeserializeObject<List<MeterEnergyList>>(serializedMeterEnergy);
        }
        else
        {
            var mm = await _meterServiceClient.GetMetersEnergy(date);

            meterEnergyList = mm
                .GroupBy(g => g.MeterId)
                .Select(s => new MeterEnergyList
                {
                    MeterId = s.Key,
                    Anomal = false,
                    DailyEnergy = s.Select(x => new DailyEnergyEntity
                    {
                        Hour = x.RecordDate.ToDateTimeOffset().Hour,
                        ActiveEnergy = Math.Abs(x.EnergyActiveExport - x.EnergyActiveImport) / 1000000,
                    }).OrderBy(o => o.Hour).ToList()
                })
                .ToList();


            foreach (var m in meterEnergyList)
            {
                if (m.DailyEnergy.Count() < 24)
                    m.Anomal = true;

                var meter = meters.FirstOrDefault(w => w.MeterId == m.MeterId);
                if (meter is not null)
                {
                    m.SerialNumber = meter.SerialNumber;
                    m.Name = meter.Name;
                    m.StationName = meter.StationName;
                    m.StationId = meter.StationId.Value;
                }
            }

            serializedMeterEnergy = JsonConvert.SerializeObject(meterEnergyList);
            encodedMeterEnergy = Encoding.UTF8.GetBytes(serializedMeterEnergy);

            var option = new DistributedCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromHours(1))
                .SetAbsoluteExpiration(DateTime.Now.AddHours(24));

            await _cache.SetAsync(casheKey, encodedMeterEnergy, option, cancellationToken);
        }

        return meterEnergyList.OrderBy(o => o.StationId).ToList();
    }

    public async Task<List<MeterEnergyList>> GetTransformersMeterEnergyFromCacheByDate(DateTime date, CancellationToken cancellationToken)
    {
        var meters = await GetMetersFromCash(cancellationToken);

        string casheKey = $"TransformersMeterEnergy-{date.Date.Ticks}";
        List<MeterEnergyList> meterEnergyList = null;
        string serializedMeterEnergy;
        var encodedMeterEnergy = await _cache.GetAsync(casheKey, cancellationToken);


        if (encodedMeterEnergy is not null)
        {
            serializedMeterEnergy = Encoding.UTF8.GetString(encodedMeterEnergy);
            meterEnergyList = JsonConvert.DeserializeObject<List<MeterEnergyList>>(serializedMeterEnergy);
        }
        else
        {
            var me = await _meterServiceClient.GetMetersEnergy(date);

            meterEnergyList = me.Where(w => w.ToolTypeId == 1)
                .GroupBy(g => g.MeterId)
                .Select(s => new MeterEnergyList
                {
                    MeterId = s.Key,
                    Anomal = false,
                    DailyEnergy = s.Select(x => new DailyEnergyEntity
                    {
                        Hour = x.RecordDate.ToDateTimeOffset().Hour,
                        ActiveEnergy = Math.Abs(x.EnergyActiveExport - x.EnergyActiveImport) / 1000000,
                    }).OrderBy(o => o.Hour).ToList()
                })
                .ToList();


            foreach (var m in meterEnergyList)
            {
                if (m.DailyEnergy.Count() < 24)
                    m.Anomal = true;

                var meter = meters.FirstOrDefault(w => w.MeterId == m.MeterId);
                if (meter is not null)
                {
                    m.SerialNumber = meter.SerialNumber;
                    m.Name = meter.Name;
                    m.StationName = meter.StationName;
                    m.StationId = meter.StationId.Value;
                }
            }

            serializedMeterEnergy = JsonConvert.SerializeObject(meterEnergyList);
            encodedMeterEnergy = Encoding.UTF8.GetBytes(serializedMeterEnergy);

            var option = new DistributedCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromHours(1))
                .SetAbsoluteExpiration(DateTime.Now.AddHours(24));

            await _cache.SetAsync(casheKey, encodedMeterEnergy, option, cancellationToken);
        }

        return meterEnergyList.OrderBy(o => o.StationId).ToList();
    }

    public async Task<List<MeterDto>> GetMetersFromCash(CancellationToken cancellationToken)
    {
        string casheKey = "Meters";
        List<MeterDto> meters = new List<MeterDto>();

        string serializedMeters;
        var encodedMeters = await _cache.GetAsync(casheKey, cancellationToken);

        if (encodedMeters is not null)
        {
            serializedMeters = Encoding.UTF8.GetString(encodedMeters);
            meters = JsonConvert.DeserializeObject<List<MeterDto>>(serializedMeters);
        }
        else
        {
            var mm = await _meterServiceClient.GetMeters();
            foreach (var m in mm)
            {
                MeterDto meter = new MeterDto()
                {
                    MeterId = m.MeterId,
                    SerialNumber = m.SerialNumber,
                    Name = m.Name,
                    StationId = m.StationId,
                    StationName = m.StationName,
                    Active = m.Active,
                    Dismount = m.Dismount,
                    StartOperationDate = m.StartOperationDate.ToDateTimeOffset(),
                    EndOperationDate = m.EndOperationDate.ToDateTimeOffset(),
                };
                meters.Add(meter);
            }

            serializedMeters = JsonConvert.SerializeObject(meters);
            encodedMeters = Encoding.UTF8.GetBytes(serializedMeters);

            var option = new DistributedCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromHours(1))
                .SetAbsoluteExpiration(DateTime.Now.AddHours(24));

            await _cache.SetAsync(casheKey, encodedMeters, option, cancellationToken);
        }
        return meters;
    }
}
