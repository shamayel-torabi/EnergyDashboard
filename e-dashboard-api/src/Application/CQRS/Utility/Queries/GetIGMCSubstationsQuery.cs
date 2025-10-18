using MediatR;
using EnergyDashboard.Application.Interfaces;
using EnergyDashboard.Application.Models;

namespace EnergyDashboard.Application.Utility.Queries;

public class GetIGMCSubstationsQuery : IRequest<IEnumerable<ValueLabel>>
{
}

public class GetIGMCSubstationsQueryHandler : IRequestHandler<GetIGMCSubstationsQuery, IEnumerable<ValueLabel>>
{
    private readonly IMeterEnergyService _meterEnergyService;

    public GetIGMCSubstationsQueryHandler(IMeterEnergyService meterEnergyService)
    {
        _meterEnergyService = meterEnergyService;
    }
    public async Task<IEnumerable<ValueLabel>> Handle(GetIGMCSubstationsQuery request, CancellationToken cancellationToken)
    {
        var mm = await _meterEnergyService.GetMetersFromCash(cancellationToken);

        var meters = mm
            .OrderBy(o => o.StationName)
            .GroupBy(g => g.StationId)
            .Select(s => s.First())
            .ToList();

        var substations = meters
            .Select(ss => new ValueLabel { Label = ss.StationName, Value = ss.StationId.Value.ToString() })
            .ToList();

        var subs = substations.ToList();

        foreach (var st in subs)
        {
            if (string.IsNullOrEmpty(st.Label) || string.IsNullOrEmpty(st.Value))
                substations.Remove(st);
        }

        substations.Add(new ValueLabel
        {
            Value = "0",
            Label = "نامعلوم",
        });

        return substations.OrderBy(o => o.Value);
    }
}
