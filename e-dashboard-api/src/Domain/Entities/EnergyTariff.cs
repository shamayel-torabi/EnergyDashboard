
using Domain.Common;

namespace EnergyDashboard.Domain.Entities;

public class EnergyTariff : Entity<Guid>
{
    private EnergyTariff(
        Guid id,
        string name,
        DateTime startDate,
        DateTime endDate) : base(id)
    {
        Name = name;
        StartDate = startDate;
        EndDate = endDate;
    }

    public string Name { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }

    protected readonly List<EnergyTariffRate> _energyTariffRates = new();
    public IReadOnlyCollection<EnergyTariffRate> EnergyTariffRates => _energyTariffRates.AsReadOnly();

    public void Update(
        string name,
        DateTime startDate,
        DateTime endDate,
        IEnumerable<EnergyTariffRate> energyTariffRates)
    {
        Name = name;
        StartDate = startDate;
        EndDate = endDate;

        _energyTariffRates.Clear();
        foreach (var etr in energyTariffRates)
        {
            var e = EnergyTariffRate.Create(etr.Id, etr.EnergyTariffId, etr.StartTime, etr.EndTime, etr.Rate);
            _energyTariffRates.Add(e);
        }
    }

    public static EnergyTariff Create(
        Guid id,
        string name,
        DateTime startDate,
        DateTime endDate,
        IEnumerable<EnergyTariffRate> energyTariffRates)
    {
        Ensure.NotEmpty(id, "id cannot be empty", nameof(id));
        Ensure.NotNull(name, "Name cannot be null or empty", nameof(name));
        Ensure.DateInRange(startDate, endDate, "endDate must be greater than startDate", nameof(EnergyTariff));

        var energyTariff = new EnergyTariff(id, name, startDate, endDate);

        foreach(var etr in energyTariffRates)
        {
            var e = EnergyTariffRate.Create(etr.Id, etr.EnergyTariffId, etr.StartTime, etr.EndTime, etr.Rate);
            energyTariff._energyTariffRates.Add(e);
        }

        return energyTariff;
    }
}
