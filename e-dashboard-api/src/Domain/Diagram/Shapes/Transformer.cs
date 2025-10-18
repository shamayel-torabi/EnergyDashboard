using System.Text;
using EnergyDashboard.Domain.Diagram.Util;
using EnergyDashboard.Domain.Enums;

namespace EnergyDashboard.Domain.Diagram.Shapes;

public interface ITransformerProperties : IProperties
{
    string DispachingCode { get; set; }
    string IgmcCode { get; set; }
    int ToolId { get; set; }
    double PrimaryVoltage { get; set; }
    double SecondaryVoltage { get; set; }
    double Mva { get; set; }
    TransofmerType TransofmerType { get; set; }
}

public class TransformerProperties : ITransformerProperties
{
    public string Name { get; set; }
    public double Voltage { get; set; }
    public string DispachingCode { get; set; }
    public string IgmcCode { get; set; }
    public int ToolId { get; set; }
    public double PrimaryVoltage { get; set; }
    public double SecondaryVoltage { get; set; }
    public double Mva { get; set; }
    public TransofmerType TransofmerType { get; set; }
    public TransformerProperties()
    {
        this.TransofmerType = TransofmerType.Subtransmission;
    }
}

public class Transformer : BoxedShape
{
    public Transformer() : base()
    {
        this.StrokeStylePrimary = "#000000";
        this.StrokeStyleSecondary = "#000000";
        this.Properties = new TransformerProperties();
    }
    public Transformer(string name, double x, double y) : base(name , x , y)
    {
        this.Shape = new Rectangle(x, y, 20, 50);
        this.StrokeStylePrimary = "#000000";
        this.StrokeStyleSecondary = "#000000";
        this.Properties = new TransformerProperties();
    }

    public new TransformerProperties Properties { get; set; }
    public string StrokeStylePrimary { get; set; }
    public string StrokeStyleSecondary { get; set; }
    public override string toSVG()
    {
        StringBuilder markup = new StringBuilder();
        var s = this.Shape as Rectangle;

        var dx = s.Width / 2;
        var x = s.X + s.Width / 2;
        var y = s.Y + s.Height / 2;

        markup.AppendLine($"\t<g id=\"{ this.Id }\" data-type=\"Transformer\" transform=\"rotate({ this.Shape.Angle * 90},{ x },{ y })\">\n");

        //markup.Append("\t<g id=\"" + this.Id + "\" transform=\"rotate(" + this.Shape.Angle * 90 + "," + x + "," + y + ")\">\n");
        var p1 = new Point(s.X + dx, s.Y);
        var p2 = new Point(s.X + dx, s.Y + dx);
        markup.Append(
            "\t\t<line" +
            " x1=\"" + p1.X + "\"" +
            " y1=\"" + p1.Y + "\"" +
            " x2=\"" + p2.X + "\"" +
            " y2=\"" + p2.Y + "\"" +
            " style=\"" + this.GetPrimarySvgStyles(this.StrokeStylePrimary) +
            "\"/>\n"
        );
        p1 = new Point(s.X + dx, s.Y + 2 * dx);
        markup.Append(
            "\t\t<circle" +
            " cx=\"" + p1.X + "\" cy=\"" + p1.Y + "\"" +
            " r=\"" + dx + "\"" +
            " style=\"" + this.GetPrimarySvgStyles(this.StrokeStylePrimary) +
            "\"/>\n");
        p1 = new Point(s.X + dx, s.Y + 3 * dx);
        markup.Append(
            "\t\t<circle" +
            " cx=\"" + p1.X + "\" cy=\"" + p1.Y + "\"" +
            " r=\"" + dx + "\"" +
            " style=\"" + this.GetPrimarySvgStyles(this.StrokeStyleSecondary) +
            "\"/>\n");

        p1 = new Point(s.X + dx, s.Y + 4 * dx);
        p2 = new Point(s.X + dx, s.Y + 5 * dx);
        markup.Append(
            "\t\t<line" +
            " x1=\"" + p1.X + "\"" +
            " y1=\"" + p1.Y + "\"" +
            " x2=\"" + p2.X + "\"" +
            " y2=\"" + p2.Y + "\"" +
            " style=\"" + this.GetPrimarySvgStyles(this.StrokeStyleSecondary) +
            "\"/>\n"
        );

        if (this.Texts != null && this.Texts.Count != 0)
            foreach (var t in this.Texts)
                markup.AppendLine(t.toSVG());

        markup.AppendLine("\t</g>");
        return markup.ToString();
    }

    private string GetPrimarySvgStyles(string str)
    {
        StringBuilder style = new StringBuilder();
        string stroke = Utilility.GetSvgColorString("stroke", str);
        string strokeWidth = this.Style.StrokeWidth.ToString();
        string s = "stroke-width: " + strokeWidth + "; ";
        style.Append(stroke);
        style.Append(s);

        string fill = Utilility.GetSvgColorString("fill", this.Style.FillStyle);
        style.Append(fill);

        string opacity = this.Style.Opacity.ToString();
        string o = "opacity: " + opacity + ";";
        style.Append(o);
        return style.ToString();
    }
}
