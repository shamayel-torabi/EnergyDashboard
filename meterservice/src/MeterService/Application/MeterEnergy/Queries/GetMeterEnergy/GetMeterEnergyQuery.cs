using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Application.Common.Exceptions;
using MeterService.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MeterService.Application.MeterEnergy.Queries;

public sealed record GetMeterEnergyQuery : IRequest<IEnumerable<MeterEnergyDTO>>
{
    public DateTime RecordDate { get; set; }
    public string SerialNumber { get; set; }
}

public sealed class GetMeterEnergyQueryHandler : IRequestHandler<GetMeterEnergyQuery, IEnumerable<MeterEnergyDTO>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetMeterEnergyQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IEnumerable<MeterEnergyDTO>> Handle(GetMeterEnergyQuery request, CancellationToken cancellationToken)
    {
        var start = request.RecordDate.Date;
        var end = start.AddDays(1);

        int meterId = await GetMeterId(request.SerialNumber);

        return await _context.MeterEnergys
            .Where(w => w.MeterId == meterId && w.RecordDate >= start && w.RecordDate < end)
            .OrderBy(x => x.RecordDate)
            .ProjectTo<MeterEnergyDTO>(_mapper.ConfigurationProvider)
            .ToListAsync();
    }

    private async Task<int> GetMeterId(string serialNumber)
    {
        var meter = await _context.Meters.FirstOrDefaultAsync(x => x.SerialNumber == serialNumber);

        if (meter == null)
            throw new NotFoundException($"Meter with SerialNumber: {serialNumber} not fount");
        else 
            return meter.MeterId;
    }
}
