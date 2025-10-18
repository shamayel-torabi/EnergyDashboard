
using MessageBus;

namespace MeterService.Application.Models;

public record HourEnergy {
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
}
public sealed class MeterEnergyMessage : IMessage
{
    public MeterEnergyMessage()
    {
        HourEnergys = new();
    }

    public int MeterId { get; set; }
    public string SerialNumber { get; set; }
    public DateTime RecordDate { get; set; }
    public List<HourEnergy> HourEnergys { get; set; }
    public bool Anomaly { get; set; }
}
