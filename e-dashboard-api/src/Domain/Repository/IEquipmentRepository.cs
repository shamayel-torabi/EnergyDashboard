using EnergyDashboard.Domain.Entities;
namespace EnergyDashboard.Domain.Repository;

public interface IEquipmentRepository : IRepository<Guid, Equipment>
{
    Task<IEnumerable<GeneratorFeederEquipment>> GeneratorFeedersEquipmentAsync(CancellationToken cancellationToken = default);
}
