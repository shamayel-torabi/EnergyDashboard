using Microsoft.EntityFrameworkCore;
using MeterService.Application.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;

namespace MeterService.Application.MeterEnergy.Queries;

public sealed record GetMetersEnergyQuery : IRequest<IEnumerable<MeterEnergyDTO>>
{
    public DateTime RecordDate { get; set; }
}

public sealed class GetMeterEnergysQueryHandler : IRequestHandler<GetMetersEnergyQuery, IEnumerable<MeterEnergyDTO>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetMeterEnergysQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IEnumerable<MeterEnergyDTO>> Handle(GetMetersEnergyQuery request, CancellationToken cancellationToken)
    {
        var start = request.RecordDate.Date;
        var end = start.AddDays(1);

        var entitys = await _context.MeterEnergys
            .Where(w => w.RecordDate >= start && w.RecordDate < end)
            .ProjectTo<MeterEnergyDTO>(_mapper.ConfigurationProvider)
            .ToListAsync();

        return entitys;
    }
}
