
namespace MeterFailService.Domain.Entities;

public sealed record HourEnergy {
    public HourEnergy(int hour, decimal activeExport, decimal activeImport, decimal reactiveExport, decimal reactiveImport)
    {
        Hour = hour;
        ActiveExport = activeExport;
        ActiveImport = activeImport;
        ReactiveExport = reactiveExport;
        ReactiveImport = reactiveImport;
    }

    public int Hour { get; private set; }
    public decimal ActiveExport { get; private set; }
    public decimal ActiveImport { get; private set; }
    public decimal ReactiveExport { get; private set; }
    public decimal ReactiveImport { get; private set; }

    public void UpdateEnergys(HourEnergy hourEnergy)
    {
        ActiveExport = hourEnergy.ActiveExport;
        ActiveImport = hourEnergy.ActiveImport;
        ReactiveExport = hourEnergy.ReactiveExport;
        ReactiveImport = hourEnergy.ReactiveImport;
    }
}
public sealed class MeterEnergyEntity
{
    public MeterEnergyEntity()
    {
        HourEnergys = Enumerable.Range(0, 24).Select(s => new HourEnergy(s, 0, 0, 0, 0)).ToList();
    }

    public Guid Id {get; set;}
    public int MeterId { get; set; }
    public string SerialNumber { get; set; }
    public DateTime RecordDate { get; set; }
    public List<HourEnergy> HourEnergys { get; set; }
    public bool Anomaly { get; set; }

    public static MeterEnergyEntity Create(Guid id, string serialNumber, DateTime recordDate, List<HourEnergy> hourEnergys)
    {
        var entity = new MeterEnergyEntity();
        entity.Id = id;
        entity.SerialNumber = serialNumber;
        entity.RecordDate = recordDate.Date;
        entity.Update(hourEnergys);

        return entity;
    }

    public void Update(List<HourEnergy> hourEnergys)
    {
        foreach (HourEnergy hourEnergy in hourEnergys)
        {
            var a = HourEnergys.FirstOrDefault(x => x.Hour == hourEnergy.Hour);
            a?.UpdateEnergys(hourEnergy);
        }
        Anomaly = hourEnergys.Count < 24 ? true : false;
    }
}
