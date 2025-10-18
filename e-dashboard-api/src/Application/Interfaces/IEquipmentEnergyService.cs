
namespace EnergyDashboard.Application.Interfaces;

public interface IEquipmentEnergyService
{
    Task UpdateEquipmentEnergy(Guid equipmentId, DateTimeOffset date, bool update);
    Task UpdateEquipmentsEnergy(DateTimeOffset date, bool update);
    Task UpdateEquipmentDailyEnergy(Guid equipmentId, DateTimeOffset date, bool update);
    Task UpdateEquipmentsDailyEnergy(DateTimeOffset date, bool update);
    Task UpdateSubstationDailyEnergy(Guid substationId, DateTimeOffset date, bool update);
    Task UpdateSubstationEquipmentEnergy(Guid substationId, DateTimeOffset date, bool update);
    Task ReadFailedMeters();
}
