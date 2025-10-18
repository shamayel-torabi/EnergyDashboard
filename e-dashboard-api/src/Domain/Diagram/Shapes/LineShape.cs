using System.Text;

namespace EnergyDashboard.Domain.Diagram.Shapes;

public class LineShape: DrawingShapeBase
{
    public LineShape() : base()
    {
        this.Shape = new Line(new Point(0, 0), new Point(0, 0));
    }
    public LineShape(string name) : base(name)
    {
        this.Shape = new Line(new Point(0, 0), new Point(0, 0));
    }
    public override string toSVG()
    {
        StringBuilder markup = new StringBuilder();
        var s = this.Shape as Line;

        markup.Append(
            "\t\t<line id=\"" + this.Id + "\"" +
            " x1=\"" + s.P1.X + "\"" +
            " y1=\"" + s.P1.Y + "\"" +
            " x2=\"" + s.P2.X + "\"" +
            " y2=\"" + s.P2.Y + "\"" +
            " style=\"" + Util.Utilility.GetSvgStyles(this.Style) + "\"/>\n");

        return markup.ToString();
    }
}
