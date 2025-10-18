using MediatR;
using EnergyDashboard.Application.Interfaces;

namespace EnergyDashboard.Application.Meters.Querie;

public class GetSubstationsQuery : IRequest<IEnumerable<SubstationVm>>
{
}

public class GetSubstationsQueryHandler : IRequestHandler<GetSubstationsQuery, IEnumerable<SubstationVm>>
{
    private readonly IMeterEnergyService _meterEnergyService;
    public GetSubstationsQueryHandler(IMeterEnergyService meterEnergyService)
    {
        _meterEnergyService = meterEnergyService;
    }

    public async Task<IEnumerable<SubstationVm>> Handle(GetSubstationsQuery request, CancellationToken cancellationToken)
    {
        var mm = await _meterEnergyService.GetMetersFromCash(cancellationToken);

        var meters = mm
            .OrderBy(o => o.StationName)
            .GroupBy(g => g.StationId)
            .Select(s => s.First())
            .ToList();  

        var substations = meters
            .Select(ss => new SubstationVm { Label = ss.StationName, Value = ss.StationId.Value })
            .ToList();

        var subs = substations.ToList();

        foreach (var st in subs)
        {
            if (string.IsNullOrEmpty(st.Label) || st.Value is null)
                substations.Remove(st);
        }

        return substations;
    }
}

public class SubstationVm
{
    public int? Value { get; set; }
    public string Label { get; set; }
}
