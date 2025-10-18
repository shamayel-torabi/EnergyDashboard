using Domain.Common;
using EnergyDashboard.Domain.Diagram.Shapes;
using EnergyDashboard.Domain.ValueObjects;

namespace EnergyDashboard.Domain.Entities;

public abstract class Equipment : Entity<Guid>
{
    internal Equipment(Guid id):base(id)
    {
        EnergyProfiles = new List<EnergyProfile>();
        DailyEnergys = new List<DailyEnergy>();
        EquipmentOperations = new List<EquipmentOperation>();
        MeterReadingFailures = new List<MeterReadingFailure>();
    }

    public string Type { get; protected set; }
    public string Name { get; protected set; }
    public Ratio CTRatio { get; protected set; }
    public Ratio PTRatio { get; protected set; }
    public double Voltage { get; protected set; }
    public bool Reverse { get; protected set; }
    public bool Virtual { get; protected set; } = false;

    public Guid CtId { get; protected set; }
    public Guid? BusbarId { get; protected set; }
    public string BusbarName { get; protected set; }
    public int? IGMCToolId { get; protected set; }
    public string IGMCCode { get; protected set; }
    public string DispatchingCode { get; protected set; }

    public Guid AreaId { get; protected set; }
    public Guid ZoneId { get; protected set; }

    public Guid SubstationId { get; protected set; }
    public Substation Substation { get; protected set; }


    protected readonly List<EquipmentMeter> _equipmentMeters = new();
    public IReadOnlyCollection<EquipmentMeter> EquipmentMeters => _equipmentMeters.AsReadOnly();

    public virtual ICollection<EnergyProfile> EnergyProfiles { get; set; }
    public virtual ICollection<DailyEnergy> DailyEnergys { get; set; }
    public virtual ICollection<EquipmentOperation> EquipmentOperations { get; set; }
    public virtual ICollection<MeterReadingFailure> MeterReadingFailures { get; set; }

    public string GetMeterSerialNumber()
    {
        var em = EquipmentMeters.FirstOrDefault(x => x.Active);

        if (em is not null)
            return em.SerialNumber;

        return string.Empty;
    }

    public EquipmentMeter GetActiveMeter()
    {
        return EquipmentMeters.FirstOrDefault(x => x.Active);
    }

    public EquipmentMeter GetCurrentMeter(DateTimeOffset date)
    {
        return EquipmentMeters.FirstOrDefault(x => x.MountDate <= date && date < x.DismountDate);
    }

    public override string ToString()
    {
        return $"Id:{Id}, Name:{Name}";
    }
}
