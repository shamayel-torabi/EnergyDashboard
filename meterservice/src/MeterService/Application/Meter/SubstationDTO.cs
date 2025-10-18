namespace MeterService.Application.Meter;

public sealed record SubstationDTO
{
    public int? Value { get; init; }
    public string Label { get; init; }
}
