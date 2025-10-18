using System.Text;
using System.Text.Json;
using AutoMapper;
using MediatR;
using MeterService.gRPC.Services;
using Microsoft.Extensions.Caching.Distributed;

namespace EnergyDashboard.Application.Meters.Commands;

public class UpdateMeterTableCommand : IRequest
{
}

public class UpdateMeterTableCommandHandler : IRequestHandler<UpdateMeterTableCommand>
{
    private readonly IMeterClient _meterService;
    private readonly IDistributedCache _cache;
    private readonly IMapper _mapper;

    public UpdateMeterTableCommandHandler(
        IMeterClient meterService,
        IDistributedCache cache,
        IMapper mapper)
    {
        _meterService = meterService;
        _cache = cache;
        _mapper = mapper;
    }

    public async Task Handle(UpdateMeterTableCommand request, CancellationToken cancellationToken)
    {
        await _meterService.UpdateMeterTable();
        await CacheMeters(cancellationToken);
    }

    private async Task CacheMeters(CancellationToken cancellationToken)
    {
        string casheKey = "Meters";
        var mtrs = await _meterService.GetMeters();
        List<MeterDto> meters = new List<MeterDto>();

        foreach (var mtr in mtrs)
        {
            MeterDto meter = new MeterDto()
            {
                MeterId = mtr.MeterId,
                SerialNumber = mtr.SerialNumber,
                Name = mtr.Name,
                StationId = mtr.StationId ,
                StationName = mtr.StationName,
                Active = mtr.Active,
                Dismount = mtr.Dismount,
                StartOperationDate = mtr.StartOperationDate.ToDateTimeOffset(),
                EndOperationDate = mtr.EndOperationDate.ToDateTimeOffset(),
            };
            meters.Add(meter);
        }

        string serializedMeters = JsonSerializer.Serialize(meters);
        byte[] encodedMeters = Encoding.UTF8.GetBytes(serializedMeters);
        DistributedCacheEntryOptions option = new DistributedCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(5.0)).SetAbsoluteExpiration(DateTime.Now.AddHours(12.0));
        await _cache.SetAsync(casheKey, encodedMeters, option, cancellationToken);
    }
}
