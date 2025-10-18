using Domain.Common;
using EnergyDashboard.Domain.Diagram;

namespace EnergyDashboard.Domain.Entities;

public class Area : Entity<Guid>
{
    public Area(Guid id, Guid networkId) : base(id)
    {
        Ensure.NotEmpty(id, "id cannot be empty", nameof(id));
        Ensure.NotEmpty(networkId, "The networkId is required.", nameof(networkId));

        Zones = new List<Zone>();
        Attributes = "{\"data-type\":\"area\"}";
        NetworkId = networkId;
    }

    public string Attributes { get;}
    public string Name { get; private set; }
    public DiagramModel Diagram { get; private set; }
    public Guid NetworkId { get; private set; }

    public virtual ICollection<Zone> Zones { get; set; }

    public void Update(DiagramModel areaDiagram)
    {
        Ensure.NotNull(areaDiagram, "areaDiagram cannot be null", nameof(areaDiagram));
        Ensure.NotEmpty(areaDiagram.Properties.Title, "Area name cannot be Null", nameof(areaDiagram.Properties.Title));

        Name = areaDiagram.Properties.Title;
        Diagram = areaDiagram;
    }

    public override string ToString()
    {
        return $"Id:{Id}, Name:{Name}";
    }
}
