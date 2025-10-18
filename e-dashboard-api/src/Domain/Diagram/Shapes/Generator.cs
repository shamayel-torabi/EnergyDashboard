using System.Text;

namespace EnergyDashboard.Domain.Diagram.Shapes;

public interface IGeneratorProperties : IProperties
{
    string DispachingCode { get; set; }
    string IgmcCode { get; set; }
    int ToolId { get; set; }
    double Capacity { get; set; }
    double NominalCapacity { get; set; }
    int GenSize { get; set; }
    int GenType { get; set; }
    int Operator { get; set; }
}

public class GeneratorProperties : IGeneratorProperties
{
    public string Name { get; set; }
    public double Voltage { get; set; }
    public string DispachingCode { get; set; }
    public string IgmcCode { get; set; }
    public int ToolId { get; set; }
    public double Capacity { get; set; }
    public double NominalCapacity { get; set; }
    public int GenSize { get; set; }
    public int GenType { get; set; }
    public int Operator { get; set; }

    public GeneratorProperties()
    {
    }
}

public class Generator : BoxedShape
{
    public Generator() : base()
    {
        this.Properties = new GeneratorProperties();
    }
    public Generator(string name, double x, double y) : base(name , x , y)
    {
        this.Shape = new Rectangle(x, y, 20, 30);
        this.Properties = new GeneratorProperties();
    }

    public new GeneratorProperties Properties { get; set; }

    public override string toSVG()
    {
        StringBuilder markup = new StringBuilder();
        var s = this.Shape as Rectangle;

        var dx = s.Width / 2;
        var x = s.X + s.Width / 2;
        var y = s.Y + s.Height / 2;

        markup.AppendLine($"\t<g id=\"{ this.Id }\" data-type=\"Generator\" transform=\"rotate({ this.Shape.Angle * 90},{ x },{ y })\">\n");

        //markup.Append("\t<g id=\"" + this.Id + "\" transform=\"rotate(" + this.Shape.Angle * 90 + "," + x + "," + y + ")\">\n");
        var p1 = new Point(s.X + dx, s.Y);
        var p2 = new Point(s.X + dx, s.Y + dx);
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
