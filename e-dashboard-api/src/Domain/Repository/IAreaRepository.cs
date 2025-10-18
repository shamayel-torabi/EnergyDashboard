using EnergyDashboard.Domain.Diagram;
using EnergyDashboard.Domain.Entities;

namespace EnergyDashboard.Domain.Repository;

public interface IAreaRepository : IRepository<Guid, Area>
{
    Task UpdateDiagram(Guid id, DiagramModel areaDiagram, CancellationToken cancellationToken = default);
}
