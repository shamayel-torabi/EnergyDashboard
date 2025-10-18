using MediatR;
using EnergyDashboard.Domain.Repository;

namespace EnergyDashboard.Application.MeterReadingFailures.Queries;

public class GetMeterFailuresQuery : IRequest<IEnumerable<MeterReadingFailureList>>
{
    public Guid Id { get; set; }
}

public class GetMeterFailuresQueryHandler : IRequestHandler<GetMeterFailuresQuery, IEnumerable<MeterReadingFailureList>>
{
    private readonly IMeterReadingFailureRepository _meterReadingFailureRepository;

    public GetMeterFailuresQueryHandler(IMeterReadingFailureRepository meterReadingFailureRepository)
    {
        _meterReadingFailureRepository = meterReadingFailureRepository;
    }

    public async Task<IEnumerable<MeterReadingFailureList>> Handle(GetMeterFailuresQuery request, CancellationToken cancellationToken)
    {
        var meterReadingFailures = await _meterReadingFailureRepository.GetMeterReadingFailuresWithEquipment(cancellationToken);

        var entitys = meterReadingFailures
            .Select(s => new
            {
                RecordDate = s.RecordDate,
                EquipmentId = s.EquipmentId,
                EquipmentName = s.Equipment.Name,
                SubstationName = s.Equipment.Substation.Name
            })
            .GroupBy(g => g.SubstationName)
            .Select(s => new MeterReadingFailureList
            {
                Substation = s.Key,
                Items = s.Select(x => new MeterReadingFailureItem
                {
                    RecordDate = x.RecordDate,
                    EquipmentId = x.EquipmentId,
                    EquipmentName = x.EquipmentName
                }).OrderBy(o => o.RecordDate).OrderBy(o => o.EquipmentName).ToList()
            }).OrderBy(o => o.Substation).ToList();

        return entitys;
    }
}


public class MeterReadingFailureItem
{
    public DateTime RecordDate { get; set; }
    public Guid EquipmentId { get; set; }
    public string EquipmentName { get; set; }
}

public class MeterReadingFailureList
{
    public string Substation { get; set; }
    public IEnumerable<MeterReadingFailureItem> Items { get; set; }

}

