using System.Text;

namespace EnergyDashboard.Domain.Diagram.Shapes;

public interface ILoadFeederProperties : IProperties
{
    string DispachingCode { get; set; }
    string IgmcCode { get; set; }
    int ToolId { get; set; }
    double MaxDemand { get; set; }
    int LoadFeederType { get; set; }
    int LoadFeederSubType { get; set; }
}

public class LoadFeederProperties : ILoadFeederProperties
{
    public string Name { get; set; }
    public double Voltage { get; set; }
    public string DispachingCode { get; set; }
    public string IgmcCode { get; set; }
    public int ToolId { get; set; }
    public double MaxDemand { get; set; }
    public int LoadFeederType { get; set; }
    public int LoadFeederSubType { get; set; }
    public LoadFeederProperties()
    {
    }
}

public class LoadFeeder : BoxedShape
{
    public LoadFeeder()
    {
        this.Properties = new LoadFeederProperties();
    }
    public LoadFeeder(string name, double x, double y) : base(name, x, y)
    {
        this.Shape = new Rectangle(x, y, 20, 20);
        this.Properties = new LoadFeederProperties();
    }
    public new LoadFeederProperties Properties { get; set; }

    public override string toSVG()
    {
        StringBuilder markup = new StringBuilder();
        var s = this.Shape as Rectangle;
        var d = s.Width / 2;
        var x = s.X + s.Width / 2;
        var y = s.Y + s.Height / 2;

        markup.AppendLine($"\t<g id=\"{this.Id}\" data-type=\"LoadFeeder\" transform=\"rotate({this.Shape.Angle * 90},{x},{y})\">\n");

        //markup.Append("\t<g id=\"" + this.Id  + "\" transform=\"rotate(" + this.Shape.Angle * 90 + "," + x + "," + y + ")\">\n");
        var p1 = new Point(s.X + d, s.Y);
        var p2 = new Point(s.X + d, s.Y + 3 * d);
        markup.Append(
            "\t\t<line" +
            " x1=\"" + p1.X + "\"" +
            " y1=\"" + p1.Y + "\"" +
            " x2=\"" + p2.X + "\"" +
            " y2=\"" + p2.Y + "\"" +
            " style=\"" + Util.Utilility.GetSvgStyles(this.Style) +
            "\"/>\n"
        );

        p1 = new Point(s.X, s.Y + 2 * d);
        p2 = new Point(s.X + d, s.Y + 3 * d);
        markup.Append(
            "\t\t<line" +
            " x1=\"" + p1.X + "\"" +
            " y1=\"" + p1.Y + "\"" +
            " x2=\"" + p2.X + "\"" +
            " y2=\"" + p2.Y + "\"" +
            " style=\"" + Util.Utilility.GetSvgStyles(this.Style) +
            "\"/>\n"
        );

        p1 = new Point(s.X + d, s.Y + 3 * d);
        p2 = new Point(s.X + 2 * d, s.Y + 2 * d);
        markup.Append(
            "\t\t<line" +
            " x1=\"" + p1.X + "\"" +
            " y1=\"" + p1.Y + "\"" +
            " x2=\"" + p2.X + "\"" +
            " y2=\"" + p2.Y + "\"" +
            " style=\"" + Util.Utilility.GetSvgStyles(this.Style) +
            "\"/>\n"
        );

        p1 = new Point(s.X + 2 * d, s.Y + 2 * d);
        p2 = new Point(s.X, s.Y + 2 * d);
        markup.Append(
            "\t\t<line" +
            " x1=\"" + p1.X + "\"" +
            " y1=\"" + p1.Y + "\"" +
            " x2=\"" + p2.X + "\"" +
            " y2=\"" + p2.Y + "\"" +
            " style=\"" + Util.Utilility.GetSvgStyles(this.Style) +
            "\"/>\n"
        );

        if (this.Texts != null && this.Texts.Count != 0)
            foreach (var t in this.Texts)
                markup.AppendLine(t.toSVG());

        markup.AppendLine("\t</g>");
        return markup.ToString();
    }
}
