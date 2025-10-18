
using Domain.Common;

namespace EnergyDashboard.Domain.Exceptions;

public static class DomainErrors
{
    public static class General
    {
        public static Error UnProcessableRequest => new Error("General.UnProcessableRequest", "The server could not process the request.");
        public static Error ServerError => new Error("General.ServerError", "سرور با یک خطا مواجه شده است.");
    }

    public static class DiagramModel
    {
        public static Error BadNetworkConnection => new Error("DiagramModel.Error", "اتصال تجهیزات کامل نیست !");
        public static Error BadEquipmentType => new Error("EquipmentType.Error", "این نوع تجهیز وجود ندارد");
    }
}
