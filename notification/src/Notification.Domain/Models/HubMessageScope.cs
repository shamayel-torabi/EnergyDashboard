
namespace Notification.Domain;

public enum HubMessageScope : byte
{
    General = 0,
    UpdateMeterTable = 1,
    UpdateEquipmentEnergyTable = 2,
    UploadMeterFile = 3,
}
