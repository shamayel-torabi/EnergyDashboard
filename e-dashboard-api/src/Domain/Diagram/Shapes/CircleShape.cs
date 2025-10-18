

namespace EnergyDashboard.Domain.Diagram.Shapes;

public class CircleShape : DrawingShapeBase
{
    public CircleShape() : base()
    {
    }
    public CircleShape(string name, double x, double y, double radius) : base(name)
    {
        this.Shape = new Circle(x, y, radius);
    }
    public override string toSVG()
    {
        var s = this.Shape as Circle;

        string svg =
            "\t<circle id=\"" + this.Id + "\"" +
            " cx=\"" + s.X + "\" cy=\"" + s.Y + "\"" +
            " r=\"" + s.Radius + "\"" +
            " style=\"" + Util.Utilility.GetSvgStyles(this.Style) + "\"/>\n";

        return svg;
    }
}
