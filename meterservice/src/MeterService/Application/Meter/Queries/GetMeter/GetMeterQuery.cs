using Microsoft.EntityFrameworkCore;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using MeterService.Application.Interfaces;
using MeterService.Application.Meter;

namespace MeterService.Application.Meters.Queries;

public sealed record GetMeterQuery : IRequest<MeterDTO>
{
    public string SerialNumber { get; set; }
}

public sealed class GetMeterQueryHandler : IRequestHandler<GetMeterQuery, MeterDTO>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetMeterQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<MeterDTO> Handle(GetMeterQuery request, CancellationToken cancellationToken)
    {
        return await _context.Meters
                .Where(t => t.SerialNumber == request.SerialNumber)
                .ProjectTo<MeterDTO>(_mapper.ConfigurationProvider)
                .SingleOrDefaultAsync(cancellationToken);
    }
}
