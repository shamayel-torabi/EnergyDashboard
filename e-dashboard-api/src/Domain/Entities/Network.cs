
using Domain.Common;
using EnergyDashboard.Domain.Diagram;

namespace EnergyDashboard.Domain.Entities;

public class Network : Entity<Guid>
{
    public Network(Guid id) : base(id)
    {
        Ensure.NotEmpty(id, "id cannot be empty", nameof(id));

        Areas = new List<Area>();
        Attributes = "{\"data-type\":\"network\"}";
    }

    public string Attributes { get; }
    public string Name { get; private set; }
    public DiagramModel Diagram { get; private set; }

    public virtual ICollection<Area> Areas { get; set; }

    public void Update(DiagramModel networkDiagram)
    {
        Ensure.NotNull(networkDiagram, "networkDiagram cannot be null", nameof(networkDiagram));
        Ensure.NotEmpty(networkDiagram.Properties.Title, "Network name cannot be Null", nameof(networkDiagram.Properties.Title));

        Name = networkDiagram.Properties.Title;
        Diagram= networkDiagram;
    }

    public override string ToString()
    {
        return $"Id:{Id}, Name:{Name}";
    }
}
