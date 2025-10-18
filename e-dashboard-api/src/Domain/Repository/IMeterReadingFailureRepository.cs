using EnergyDashboard.Domain.Entities;

namespace EnergyDashboard.Domain.Repository;

public interface IMeterReadingFailureRepository : IRepository<Guid, MeterReadingFailure>
{
    Task<IEnumerable<MeterReadingFailure>> GetMeterReadingFailuresWithEquipment(CancellationToken cancellationToken = default);
}
