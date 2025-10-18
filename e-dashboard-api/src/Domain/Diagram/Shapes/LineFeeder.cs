using System.Text;

namespace EnergyDashboard.Domain.Diagram.Shapes;

public interface ILineFeederProperties : IProperties
{
    string DispachingCode { get; set; }
    string IgmcCode { get; set; }
    int ToolId { get; set; }
    double TransferCapacity { get; set; }
    int LineFeederType { get; set; }
}

public class LineFeederProperties : ILineFeederProperties
{
    public string Name { get; set; }
    public double Voltage { get; set; }
    public string DispachingCode { get; set; }
    public string IgmcCode { get; set; }
    public int ToolId { get; set; }
    public double TransferCapacity { get; set; }
    public int LineFeederType { get; set; }
    public LineFeederProperties()
    {
    }
}

public class LineFeeder : BoxedShape
{
    private LineFeederProperties properties;

    public LineFeeder() : base()
    {
        this.properties = new LineFeederProperties();
        this.ConnectPortId = string.Empty;
        this.ConnectShapeId = string.Empty;
    }
    public LineFeeder(string name, double x, double y) : base(name, x, y)
    {
        this.Shape = new Rectangle(x, y, 10, 20);
        this.properties = new LineFeederProperties();
        this.ConnectPortId = string.Empty;
        this.ConnectShapeId = string.Empty;

        var shape = this.Shape as Rectangle;

        var location = new Point(x, y);
        var w = shape.Width / 2;
        var h = shape.Height;

        //var h = shape.Height / 2;
        Point offset = new Point(w, h);

        AddPort("1", location, offset);

        var style = new TextStyle();
        style.TextAlign = "center"; // start, end, left, right, center
        style.TextBaseline = "bottom"; //top hanging middle alphabetic ideographic bottom central

        offset = new Point(w, -this.SelectionZoneWidth);
        AddText("Name", this.Properties.Name, location, offset, style);

        offset = new Point(w, - 26);
        AddText("P", $"P={0} Mw", location, offset, style);

        offset = new Point(w, - 14);
        AddText("Q", $"Q={0} MVar", location, offset, style);

        this.Shape.Angle = 0;
    }
    public new LineFeederProperties Properties {
        get {
            return properties;
        } 
        set {
            SetText("Name", value.Name);
            properties = value;
        } 
    }
    public string ConnectPortId { get; set; }
    public string ConnectShapeId { get; set; }        
    public override string toSVG()
    {
        StringBuilder markup = new StringBuilder();
        var s = this.Shape as Rectangle;
        var d = s.Width / 2;
        var x = s.X + s.Width / 2;
        var y = s.Y + s.Height / 2;

        markup.AppendLine($"\t<g id=\"{ this.Id }\" data-type=\"LineFeeder\" transform=\"rotate({ this.Shape.Angle * 90},{ x },{ y })\">\n");

        var p1 = new Point(s.X + d, s.Y + d);
        var p2 = new Point(s.X + d, s.Y + 4 * d);
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
        p2 = new Point(s.X + d, s.Y + d);
        markup.Append(
            "\t\t<line" +
            " x1=\"" + p1.X + "\"" +
            " y1=\"" + p1.Y + "\"" +
            " x2=\"" + p2.X + "\"" +
            " y2=\"" + p2.Y + "\"" +
            " style=\"" + Util.Utilility.GetSvgStyles(this.Style) +
            "\"/>\n"
        );

        p1 = new Point(s.X + d, s.Y + d);
        p2 = new Point(s.X + 2 * d, s.Y + 2 * d);
        markup.Append(
            "\t\t<line " +
            "x1=\"" + p1.X + "\"" +
            " y1=\"" + p1.Y + "\"" +
            " x2=\"" + p2.X + "\"" +
            " y2=\"" + p2.Y + "\"" +
            " style=\"" + Util.Utilility.GetSvgStyles(this.Style) +
            "\"/>\n"
        );


        p1 = new Point(s.X, s.Y + d);
        p2 = new Point(s.X + d, s.Y);
        markup.Append(
            "\t\t<line " +
            "x1=\"" + p1.X + "\"" +
            " y1=\"" + p1.Y + "\"" +
            " x2=\"" + p2.X + "\"" +
            " y2=\"" + p2.Y + "\"" +
            " style=\"" + Util.Utilility.GetSvgStyles(this.Style) +
            "\"/>\n"
        );

        p1 = new Point(s.X + d, s.Y);
        p2 = new Point(s.X + 2 * d, s.Y + d);
        markup.Append(
            "\t\t<line " +
            "x1=\"" + p1.X + "\"" +
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
