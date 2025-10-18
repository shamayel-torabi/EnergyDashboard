using MeterFailService.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using MeterService.gRPC.Services;
using MeterFailService.Application.Common.Core;
using Domain.Common.Result;
using Application.Common.Models;

namespace MeterFailService.Application.MeterEnergy.Queries;

public sealed record GetPaginatedMeterEnergyQuery : IQuery<Result<PaginatedList<MeterEnergyDto>>>
{
    public DateTimeOffset RecordDate { get; set; }
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 25;
    public bool Abnormal { get; set; } = false;
}

public sealed class GetPaginatedMeterEnergyQueryHandler : IQueryHandler<GetPaginatedMeterEnergyQuery, Result<PaginatedList<MeterEnergyDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMeterClient _meterReaingService;


    public GetPaginatedMeterEnergyQueryHandler(IApplicationDbContext context, IMeterClient meterReaingService)
    {
        ArgumentNullException.ThrowIfNull(context, nameof(context));
        ArgumentNullException.ThrowIfNull(meterReaingService, nameof(meterReaingService));

        _context = context;
        _meterReaingService = meterReaingService;
    }

    public async Task<Result<PaginatedList<MeterEnergyDto>>> Handle(GetPaginatedMeterEnergyQuery request, CancellationToken cancellationToken)
    {
        var meters = await _meterReaingService.GetMeters();

        DateTime start = request.RecordDate.Date;
        DateTime end = start.AddDays(1);

        var meterEnergyList = await _context.MeterEnergys
            .Where(w => w.RecordDate >= start && w.RecordDate < end)
            .ToListAsync();

        List<MeterEnergyDto> meterEnergys = new();
        foreach (var m in meterEnergyList)
        {
            var meter = meters.FirstOrDefault(w => w.SerialNumber == m.SerialNumber);

            if (meter is not null) {
                MeterEnergyDto meterEnergyDto = new()
                {
                    Id = m.Id,
                    SerialNumber = m.SerialNumber,
                    Name = meter.Name,
                    StationName = meter.StationName,
                    RecordDate = DateOnly.FromDateTime(m.RecordDate),
                    HourEnergys = m.HourEnergys.ToList(),
                    Anomaly = m.Anomaly
                };
                
                meterEnergys.Add(meterEnergyDto);
            }
        }

        meterEnergys = meterEnergys.OrderBy(w => w.StationName).ToList();

        PaginatedList<MeterEnergyDto> list;
        if (request.Abnormal)
        {
            list = PaginatedList<MeterEnergyDto>.Create(meterEnergys.Where((MeterEnergyDto w) => w.Anomaly).ToList(), request.PageIndex, request.PageSize);
        }
        else
        {
            list = PaginatedList<MeterEnergyDto>.Create(meterEnergys, request.PageIndex, request.PageSize);
        }

        return Result.Success(list);
    }
}
