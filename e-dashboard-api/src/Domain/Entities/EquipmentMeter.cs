
using Domain.Common;
using EnergyDashboard.Domain.Diagram.Shapes;

namespace EnergyDashboard.Domain.Entities;

public record EquipmentMeter
{
    public Guid EquipmentId { get; init; }
    public string SerialNumber { get; init; }
    public DateTimeOffset MountDate { get; init; }
    public DateTimeOffset DismountDate { get; init; }
    public bool Active { get; init; } = false;

    public static EquipmentMeter Create(Guid equipmentId, Meter meter)
    {
        Ensure.NotEmpty(equipmentId, "The equipmentId cannot be empty.", nameof(equipmentId));
        Ensure.NotNull(meter, "Meter must not ne Null", nameof(meter));
        Ensure.DateInRange(meter.MountDate, meter.DismountDate, "DismountDate must be greater than MountDate", "Meter DateInRange Error");

        var equipmentmeter = new EquipmentMeter
        {
            EquipmentId = equipmentId,
            SerialNumber = meter.SerialNumber,
            MountDate = meter.MountDate,
            DismountDate = meter.DismountDate,
            Active = meter.Active
        };

        return equipmentmeter;
    }
}
