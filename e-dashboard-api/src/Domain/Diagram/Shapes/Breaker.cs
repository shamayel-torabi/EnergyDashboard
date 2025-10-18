using System.Text;

namespace EnergyDashboard.Domain.Diagram.Shapes;

public interface IBreakerProperties : IProperties
{
    bool Status { get; set; }
    string DispachingCode { get; set; }
    string IgmcCode { get; set; }
}

public class BreakerProperties : IBreakerProperties
{
    public string Name { get; set; }
    public double Voltage { get; set; }
    public bool Status { get; set; }
    public string DispachingCode { get; set; }
    public string IgmcCode { get; set; }

    public BreakerProperties()
    {
    }
}

public class Breaker : BoxedLink
{
    public Breaker() : base()
    {
        this.Properties = new BreakerProperties();
   }
    public Breaker(string name , double x , double y) : base(name , x , y)
    {
        this.Properties = new BreakerProperties();
        this.Shape = new Rectangle(x , y , 10 , 20);
    }

    public new BreakerProperties Properties { get; set; }

    public override string toSVG()
    {
        StringBuilder markup = new StringBuilder();
        var s = this.Shape as Rectangle;
        var dx = s.Width / 2;
        var x = s.X + s.Width / 2;
        var y = s.Y + s.Height / 2;

        markup.AppendLine($"\t<g id=\"{ this.Id }\" data-type=\"Breaker\" transform=\"rotate({ this.Shape.Angle * 90},{ x },{ y })\">\n");

        //markup.AppendLine("\t<g id=\"" + this.Id + "\" transform=\"rotate(" + this.Shape.Angle * 90 + "," + x + "," + y + ")\">\n");
        var p1 = new Point(s.X + dx , s.Y);
        var p2 = new Point(s.X + dx , s.Y + dx);
        markup.AppendLine(
            "\t\t<line" +
            " x1=\"" + p1.X + "\"" +
            " y1=\"" + p1.Y + "\"" +
            " x2=\"" + p2.X + "\"" +
            " y2=\"" + p2.Y + "\"" +
            " style=\"" + Util.Utilility.GetSvgStyles(this.Style) +
            "\"/>"
        );

        p1 = new Point(s.X , s.Y + dx);
        markup.AppendLine(
            "\t\t<rect" +
            " x=\"" + p1.X + "\"" +
            " y=\"" + p1.Y + "\"" +
            " width=\"" + 2 * dx + "\"" +
            " height=\"" + 2 * dx + "\"" +
            " style=\"" + Util.Utilility.GetSvgStyles(this.Style) + "\"" +
            "/>");

        p1 = new Point(s.X + dx , s.Y + 3 * dx);
        p2 = new Point(s.X + dx , s.Y + 4 * dx);
        markup.AppendLine(
            "\t\t<line" +
            " x1=\"" + p1.X + "\"" +
            " y1=\"" + p1.Y + "\"" +
            " x2=\"" + p2.X + "\"" +
            " y2=\"" + p2.Y + "\"" +
            " style=\"" + Util.Utilility.GetSvgStyles(this.Style) +
            "\"/>"
        );

        if (this.Texts != null && this.Texts.Count != 0)
            foreach (var t in this.Texts)
                markup.AppendLine(t.toSVG());

        markup.AppendLine("\t</g>");
        return markup.ToString();
    }
}
