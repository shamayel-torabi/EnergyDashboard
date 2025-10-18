using Microsoft.EntityFrameworkCore;
using MeterFailService.Application.Interfaces;
using MeterFailService.Application.Common.Core;
using Domain.Common.Result;

namespace MeterFailService.Application.MeterEnergy.Queries;

public sealed record GetMetersEnergyQuery : IQuery<Result<IEnumerable<MeterEnergyDto>>>
{
    public DateTime RecordDate { get; set; }
}

public sealed class GetMeterEnergysQueryHandler : IQueryHandler<GetMetersEnergyQuery, Result<IEnumerable<MeterEnergyDto>>>
{
    private readonly IApplicationDbContext _context;

    public GetMeterEnergysQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<IEnumerable<MeterEnergyDto>>> Handle(GetMetersEnergyQuery request, CancellationToken cancellationToken)
    {
        var start = request.RecordDate.Date;
        var end = start.AddDays(1);

        var meterEnergyList = await _context.MeterEnergys
            .Where(w => w.RecordDate >= start && w.RecordDate < end)
            .ToListAsync();

        List<MeterEnergyDto> entitys = new();
        foreach (var m in meterEnergyList)
        {
            MeterEnergyDto meterEnergyDto = new()
            {
                Id = m.Id,
                SerialNumber = m.SerialNumber,
                RecordDate = DateOnly.FromDateTime(m.RecordDate),
                HourEnergys = m.HourEnergys.ToList(),
                Anomaly = m.Anomaly
            };
            
            entitys.Add(meterEnergyDto);
        }

        return Result.Success<IEnumerable<MeterEnergyDto>>(entitys);
    }
}
