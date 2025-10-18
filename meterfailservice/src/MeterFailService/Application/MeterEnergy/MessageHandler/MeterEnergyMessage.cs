using MessageBus;
using MeterFailService.Domain.Entities;

namespace MeterFailService.Application.MeterEnergy.MessageHandler;

public sealed record MeterEnergyMessage : IMessage
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
