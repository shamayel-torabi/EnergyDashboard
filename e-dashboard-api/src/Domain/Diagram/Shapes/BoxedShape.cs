
namespace EnergyDashboard.Domain.Diagram.Shapes; 
public class BoxedShape : DrawingShapeBase
{
    public BoxedShape() : base()
    {
    }
    public BoxedShape(string name, double x, double y) : base(name)
    {
        this.Shape = new Rectangle(x, y, 10, 20);
    }
    public override string toSVG()
    {
        return "";
    }
}
