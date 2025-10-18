using EnergyDashboard.Domain.Diagram;
using EnergyDashboard.Domain.Entities;

namespace EnergyDashboard.Domain.Repository;

public interface INetworkRepository : IRepository<Guid, Network>
{
    Task<IEnumerable<Network>> GetAllNetwork(CancellationToken cancellationToken = default);
    Task<Network> Export(Guid id, CancellationToken cancellationToken = default);
    Task Import(Network network, CancellationToken cancellationToken = default);
    Task UpdateDiagram(Guid id, DiagramModel networkDiagram, CancellationToken cancellationToken = default);
}
