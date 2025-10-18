using EnergyDashboard.Domain.Diagram;
using EnergyDashboard.Domain.Entities;

namespace EnergyDashboard.Domain.Repository;

public interface IZoneRepository : IRepository<Guid, Zone>
{
    Task UpdateDiagram(Guid id, DiagramModel zoneDiagram, CancellationToken cancellationToken = default);
}
