
using Domain.Common;

namespace EnergyDashboard.Domain.Entities;

public class EquipmentOperation : Entity<Guid>
{
    public DateTime DateOfOpereation { get; set; }
    public string Description { get; set; }
    public Guid UserId { get; set; }

    public int EquipmentOperationTypeId { get; set; }
    public Guid EquipmentId { get; set; }
    public Guid SubstationId { get; set; }
}
