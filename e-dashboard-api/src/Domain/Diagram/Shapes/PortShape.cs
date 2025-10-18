
using System.Text.Json.Serialization;

namespace EnergyDashboard.Domain.Diagram.Shapes;

[JsonDerivedType(typeof(PortShape), nameof(PortShape))]
public class PortShape
{
    public PortShape()
    {
        this.Id = Guid.NewGuid().ToString();
        this.Location = new Point(0, 0);
        this.IsConnected = false;
        this.Handle = 0;
        this.SelectionZoneWidth = 3;
        this.ConnectShapeId = string.Empty;
        this.Style = new Style();
        this.Style.Opacity = 1;
        this.Style.FillStyle = "#FFFFFF00";
        this.Style.StrokeStyle = "#FF0000";
        this.Style.StrokeWidth = 1;
    }
    public PortShape(string name, Point offset):this()
    {
        this.Name = name;
        this.Offset = offset;
    }

    public string Id { get; set; }
    public string Name { get; set; }
    public Point Offset { get; set; }
    public Point Location { get; set; }
    public bool IsConnected { get; set; }
    public int Handle { get; set; }
    public int SelectionZoneWidth { get; set; }
    public string ConnectShapeId { get; set; }
    public Style Style { get; set; }
}
