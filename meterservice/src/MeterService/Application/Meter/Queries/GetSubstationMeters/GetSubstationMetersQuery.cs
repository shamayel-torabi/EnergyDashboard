using MediatR;
using MeterService.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MeterService.Application.Meter;

namespace MeterService.Application.Meters.Queries;

public sealed record GetSubstationMetersQuery : IRequest<IEnumerable<MeterDTO>>
{
    public int SubstationId { get; set; }
}

public sealed class GetSubstationMetersQueryHandler : IRequestHandler<GetSubstationMetersQuery, IEnumerable<MeterDTO>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetSubstationMetersQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IEnumerable<MeterDTO>> Handle(GetSubstationMetersQuery request, CancellationToken cancellationToken)
    {
       var meters = await _context.Meters
            .Where(w => w.StationId == request.SubstationId)
            .ProjectTo<MeterDTO>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);

        return  meters;
    }
}
