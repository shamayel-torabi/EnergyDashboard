using Microsoft.EntityFrameworkCore;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using MeterService.Application.Interfaces;
using MeterService.Application.Meter;

namespace MeterService.Application.Meters.Queries;

public sealed record GetMetersQuery : IRequest<IEnumerable<MeterDTO>>
{
}

public sealed class GetMetersQueryHandler : IRequestHandler<GetMetersQuery, IEnumerable<MeterDTO>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetMetersQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IEnumerable<MeterDTO>> Handle(GetMetersQuery request, CancellationToken cancellationToken)
    {
        return await _context.Meters
            .ProjectTo<MeterDTO>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);
    }
}
