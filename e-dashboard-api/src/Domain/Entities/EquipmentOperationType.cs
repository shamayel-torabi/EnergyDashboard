using Domain.Common;
using EnergyDashboard.Domain.Enums;

namespace EnergyDashboard.Domain.Entities;

public class EquipmentOperationType:Entity<int>
{
    public EquipmentType EquipmentType { get; set; }
    public string OperationName { get; set; }
}
