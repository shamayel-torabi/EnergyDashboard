using MediatR;
using MeterService.Application.Interfaces;
using MeterService.Application.Meter;

namespace MeterService.Application.Meters.Queries;

public sealed record GetSubstationsQuery : IRequest<IEnumerable<SubstationDTO>>
{
}

public sealed class GetSubstationsQueryHandler : IRequestHandler<GetSubstationsQuery, IEnumerable<SubstationDTO>>
{
    private readonly IApplicationDbContext _context;

    public GetSubstationsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<SubstationDTO>> Handle(GetSubstationsQuery request, CancellationToken cancellationToken)
    {
        var substations = _context.Meters
            .OrderBy(o => o.StationName)
            .GroupBy(g => g.StationId)
            .Select(s => s.First())
            .AsEnumerable()
            .Select(ss => new SubstationDTO { Label = ss.StationName, Value = ss.StationId.Value })
            .ToList();

        var subs = substations.ToList();

        foreach (var st in subs)
        {
            if (st.Label == null || st.Value == null)
                substations.Remove(st);
        }


        return await Task.FromResult(substations);
    }
}
