using Domain.Common;
using EnergyDashboard.Domain.Diagram;

namespace EnergyDashboard.Domain.Entities;

public class Zone : Entity<Guid>
{
    public Zone(Guid id, Guid areaId) : base(id)
    {
        Ensure.NotEmpty(id,"id cannot be empty", nameof(id));
        Ensure.NotEmpty(areaId, "The areaId is required.", nameof(areaId));

        Substations = new List<Substation>();
        Attributes = "{\"data-type\":\"zone\"}";
        AreaId = areaId;
    }

    public string Attributes { get;}
    public string Name { get; private set; }
    public DiagramModel Diagram { get; private set; }

    public Guid AreaId { get; private set; }
    public virtual ICollection<Substation> Substations { get; set; }


    public void Update(DiagramModel zoneDiagram)
    {
        Ensure.NotNull(zoneDiagram, "zoneDiagram cannot be null", nameof(zoneDiagram));
        Ensure.NotEmpty(zoneDiagram.Properties.Title, "Zone name cannot be Null", nameof(zoneDiagram.Properties.Title));

        Name = zoneDiagram.Properties.Title;
        Diagram = zoneDiagram;
    }

    public override string ToString()
    {
        return $"Id:{Id}, Name:{Name}";
    }
}
