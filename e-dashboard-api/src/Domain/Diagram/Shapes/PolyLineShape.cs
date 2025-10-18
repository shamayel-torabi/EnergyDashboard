using System.Text;

namespace EnergyDashboard.Domain.Diagram.Shapes;

public class PolyLineShape : DrawingShapeBase
{
    public PolyLineShape() : base()
    {
        this.Shape = new PolyLine();
    }
    public PolyLineShape(string name) : base(name)
    {
        this.Shape = new PolyLine();
    }
    public override string toSVG()
    {
        StringBuilder markup = new StringBuilder();
        var s = this.Shape as PolyLine;

        markup.Append("\t<g id=\""+ this.Id+ "\">\n");

        for (int i = 0; i < s.Points.Count - 1; i++)
        {
            markup.Append(
                "\t\t<line "+
                "x1=\"" + s.Points[i].X + "\"" +
                " y1=\"" + s.Points[i].Y + "\"" +
                " x2=\"" + s.Points[i + 1].X + "\"" +
                " y2=\"" + s.Points[i + 1].Y + "\"" +
                " style=\""+ Util.Utilility.GetSvgStyles(this.Style) + "\"/>\n");
        }
        markup.Append("\t</g>\n");
        return markup.ToString();
    }
}
