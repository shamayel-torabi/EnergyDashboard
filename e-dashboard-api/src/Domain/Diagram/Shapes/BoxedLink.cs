
namespace EnergyDashboard.Domain.Diagram.Shapes;

public class BoxedLink : BoxedShape
{
    public BoxedLink() : base()
    {
        this.ConnectShapeType = string.Empty;
    }
    public BoxedLink(string name, double x, double y) : base(name, x, y)
    {
        this.ConnectShapeType = string.Empty;
    }

    public string ConnectShapeType { get; set; }
}
