using System.Text;

namespace EnergyDashboard.Domain.Diagram.Shapes;

public class Meter
{
    public int MeterId { get; set; }
    public string Name { get; set; }
    public string SerialNumber { get; set; }
    public DateTime MountDate { get; set; }
    public DateTime DismountDate { get; set; }
    public bool Active { get; set; }
}

public interface ICTProperties : IProperties
{
    string RatioCT { get; set; }
    string RatioPT { get; set; }
    bool Reverse { get; set; }
    IList<Meter> Meters { get; set; }
}


public class CTProperties : ICTProperties
{
    public CTProperties()
    {
        this.Meters = new List<Meter>();
    }

    public string Name { get; set; }
    public double Voltage { get; set; }
    public string RatioCT { get; set; }
    public string RatioPT { get; set; }
    public bool Reverse { get; set; }
    public IList<Meter> Meters { get; set; }
}

public class CT : BoxedLink
{
    public CT() : base()
    {
        this.Properties = new CTProperties();
    }
    public CT(string name, double x, double y) : base(name , x , y)
    {
        this.Properties = new CTProperties();
    }

    public new CTProperties Properties { get; set; }

    public override string toSVG()
    {
        StringBuilder markup = new StringBuilder();
        var s = this.Shape as Rectangle;

        var dx = s.Width / 2;
        var x = s.X + s.Width / 2;
        var y = s.Y + s.Height / 2;

        markup.AppendLine($"\t<g id=\"{ this.Id }\" data-type=\"CT\" transform=\"rotate({ this.Shape.Angle * 90},{ x },{ y })\">\n");

        //markup.Append("\t<g id=\"" + this.Id + "\" transform=\"rotate(" + this.Shape.Angle * 90 + "," + x + "," + y + ")\">\n");
        var p1 = new Point(s.X + dx, s.Y);
        var p2 = new Point(s.X + dx, s.Y + 4 * dx);
        markup.Append(
            "\t\t<line" +
            " x1=\"" + p1.X + "\"" +
            " y1=\"" + p1.Y + "\"" +
            " x2=\"" + p2.X + "\"" +
            " y2=\"" + p2.Y + "\"" +
            " style=\"" + Util.Utilility.GetSvgStyles(this.Style) + "\"/>\n"
        );
        p1 = new Point(s.X + dx, s.Y + 2 * dx);
        markup.Append(
            "\t\t<circle" +
            " cx=\"" + p1.X + "\" cy=\"" + p1.Y + "\"" +
            " r=\"" + dx + "\"" +
            " style=\"" + Util.Utilility.GetSvgStyles(this.Style) + "\"/>\n");


        if (this.Texts != null && this.Texts.Count != 0)
            foreach (var t in this.Texts)
                markup.AppendLine(t.toSVG());

        markup.AppendLine("\t</g>");
        return markup.ToString();
    }
}
