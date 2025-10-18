using EnergyDashboard.Domain.Diagram;
using EnergyDashboard.Domain.Entities;

namespace EnergyDashboard.Domain.Repository;

public interface ISubstationRepository : IRepository<Guid, Substation>
{
    Task<Substation> GetSubstationWithEquipments(Guid substationId, CancellationToken cancellationToken = default);
    Task UpdateDiagram(Guid id, DiagramModel substationDiagram, CancellationToken cancellationToken = default);
}
