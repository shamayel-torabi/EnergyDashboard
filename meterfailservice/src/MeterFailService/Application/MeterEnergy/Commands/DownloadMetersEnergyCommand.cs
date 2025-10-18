using MeterFailService.Application.Common.Core;
using MeterFailService.Application.Interfaces;
using MeterFailService.Domain.Entities;
using MeterService.gRPC.Services;
using Microsoft.EntityFrameworkCore;
using Domain.Common.Result;

namespace MeterFailService.Application.MeterEnergy.Commands;

public sealed record DownloadMetersEnergyCommand : ICommand<Result>
{
    public DateTimeOffset RecordDate { get; set; }
}


public sealed class DownloadMetersEnergyCommandHandler : ICommandHandler<DownloadMetersEnergyCommand, Result>
{
    private readonly IMeterClient _meterReaingService;
    private readonly IApplicationDbContext _dbContext;
    private readonly ICryptoService _cryptoService;

    public DownloadMetersEnergyCommandHandler(
        IApplicationDbContext dbContext,
        ICryptoService cryptoService,
        IMeterClient meterReaingService)
    {
        _dbContext = dbContext;
        _cryptoService = cryptoService;
        _meterReaingService = meterReaingService;
    }

    public async Task<Result> Handle(DownloadMetersEnergyCommand request, CancellationToken cancellationToken)
    {
        var metersEnergy = await _meterReaingService.GetMetersEnergy(request.RecordDate);
        var meters = await _meterReaingService.GetMeters();

        var meterEnergyList = metersEnergy.GroupBy(g => g.MeterId)
            .Select(s => new MeterEnergyEntity
            {
                MeterId = s.Key,
                RecordDate = request.RecordDate.Date,
                HourEnergys = s.Select(x => new HourEnergy(
                    x.RecordDate.ToDateTimeOffset().Hour,
                    x.EnergyActiveExport,
                    x.EnergyActiveImport,
                    x.EnergyReactiveExport,
                    x.EnergyReactiveImport
                )).OrderBy(o => o.Hour).ToList()
            }).ToList();

        foreach (var m in meterEnergyList)
        {
            var meter = meters.FirstOrDefault(w => w.MeterId == m.MeterId);

            if (meter is not null)
            {
                Guid guid = _cryptoService.GetDeterministicGuid(meter.SerialNumber, m.RecordDate.Ticks);

                if (_dbContext.MeterEnergys.Any(f => f.Id == guid))
                {
                    MeterEnergyEntity entity = await _dbContext.MeterEnergys.FirstOrDefaultAsync(f => f.Id == guid);
                    entity.Update(m.HourEnergys);
                    _dbContext.MeterEnergys.Update(entity);
                }
                else
                {
                    MeterEnergyEntity entity = MeterEnergyEntity.Create(guid, meter.SerialNumber, m.RecordDate, m.HourEnergys);
                    await _dbContext.MeterEnergys.AddAsync(entity, cancellationToken);
                }
            }
        }

        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
