using Domain.Common;

namespace MeterService.Domain.ValueObjects;

public sealed class MeterInstant: ValueObject
{
    public MeterInstant(
        string serialNumber, DateTime meterTime, 
        decimal pExport, decimal pImport,
        decimal   qExport, decimal qImport, 
        decimal voltageA, decimal voltageB, decimal voltageC, 
        decimal currentA, decimal currentB, decimal currentC,
        bool estimateStatus)
    {
        SerialNumber = serialNumber;
        MeterTime = meterTime;
        PExport = pExport;
        PImport = pImport;
        QExport = qExport;
        QImport = qImport;
        VoltageA = voltageA;
        VoltageB = voltageB;
        VoltageC = voltageC;
        CurrentA = currentA;
        CurrentB = currentB;
        CurrentC = currentC;
        EstimateStatus = estimateStatus;
    }

    public string SerialNumber { get; private set; }
    public DateTime MeterTime { get; private set; }
    public decimal PExport { get; private set; }
    public decimal PImport { get; private set; }
    public decimal QExport { get; private set; }
    public decimal QImport { get; private set; }
    public decimal VoltageA { get; private set; }
    public decimal VoltageB { get; private set; }
    public decimal VoltageC { get; private set; }
    public decimal CurrentA { get; private set; }
    public decimal CurrentB { get; private set; }
    public decimal CurrentC { get; private set; }
    public bool EstimateStatus { get; private set; }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return SerialNumber;
        yield return MeterTime;
    }
}
