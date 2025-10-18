using EnergyDashboard.Application.Models.Meters;
using EnergyDashboard.Application.Models;
using EnergyDashboard.Application.Interfaces;
using MediatR;

namespace EnergyDashboard.Application.MeterEnergys.Queries;

public class GetPaginatedMeterEnergyByDateQuery : IRequest<PaginatedList<MeterEnergyList>>
{
    public DateTimeOffset RecordDate { get; set; }
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 25;
    public bool Abnormal { get; set; } = false;
}

public class GetPaginatedMeterEnergyByDateQueryHandler : IRequestHandler<GetPaginatedMeterEnergyByDateQuery, PaginatedList<MeterEnergyList>>
{
    private readonly IMeterEnergyService _meterEnergyService;
    public GetPaginatedMeterEnergyByDateQueryHandler(IMeterEnergyService meterEnergyService)
    {
        _meterEnergyService = meterEnergyService;
    }

    public async Task<PaginatedList<MeterEnergyList>> Handle(GetPaginatedMeterEnergyByDateQuery request, CancellationToken cancellationToken)
    {
        var meterEnergyList = await _meterEnergyService.GetMeterEnergyFromCacheByDate(request.RecordDate.Date, cancellationToken);

        if (request.Abnormal)
        {
            return PaginatedList<MeterEnergyList>.Create(meterEnergyList.Where((MeterEnergyList w) => w.Anomal).ToList(), request.PageIndex, request.PageSize);
        }
        else
            return PaginatedList<MeterEnergyList>.Create(meterEnergyList, request.PageIndex, request.PageSize);
    }
}
