
using Domain.Common;

namespace EnergyDashboard.Domain.Entities;

public record EnergyTariffRate
{
    public Guid Id { get; init; }
    public Guid EnergyTariffId { get; init; }
    public TimeSpan StartTime { get; init; }
    public TimeSpan EndTime { get; init; }
    public double Rate { get; init; }

    public static EnergyTariffRate Create(
        Guid id,
        Guid energyTariffId,
        TimeSpan startTime,
        TimeSpan endTime,
        double rate)
    {
        Ensure.NotEmpty(id, "id cannot be empty", nameof(id));

        Ensure.NotEmpty(energyTariffId, "energyTariffId cannot be empty", nameof(energyTariffId));

        if (endTime <= startTime)
            throw new ArgumentException("endTime must be greater than startTime", nameof(EnergyTariffRate));

        if (rate < 0)
            throw new ArgumentException("rate must be greater than zero", nameof(rate));

        var etr = new EnergyTariffRate
        {
            Id = id,
            EnergyTariffId = energyTariffId,
            StartTime = startTime,
            EndTime = endTime,
            Rate = rate
        };

        return etr;
    }
}
