
using EnergyDashboard.Domain.Entities;

namespace EnergyDashboard.Domain.Repository;

public interface IEnergyProfileRepository : IRepository<Guid, EnergyProfile>
{
    Task<IEnumerable<EnergyProfile>> GetEquipmentEnergyByDateAsync(Guid equipmentId, DateTimeOffset date, CancellationToken cancellationToken = default);
    Task<IEnumerable<EnergyProfile>> GetEquipmentEnergyMonthlyAsync(Guid equipmentId, int year, int month, CancellationToken cancellationToken = default);
    Task<IEnumerable<EnergyProfile>> GetEquipmentEnergyProfileIntervalAsync(Guid equipmentId, DateTimeOffset startDate, DateTimeOffset endDate, CancellationToken cancellationToken = default);
    Task<IEnumerable<EnergyProfile>> GetPowerPlantEnergyAsync(int PowerplantId, DateTimeOffset date, CancellationToken cancellationToken = default);
    Task<IEnumerable<EnergyProfile>> GetSubstationEnergyAsync(Guid substationId, DateTimeOffset date, CancellationToken cancellationToken = default);
}
