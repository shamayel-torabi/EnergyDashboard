
namespace EnergyDashboard.Domain.Diagram.Shapes;

public interface IRectangleShapeProperties : IProperties
{
    int LineWidth { get; set; }
    string StrokeStyle { get; set; }
}

public class RectangleShapeProperties : IRectangleShapeProperties
{
    public string Name { get; set; }
    public double Voltage { get; set; }
    public int LineWidth { get; set; }
    public string StrokeStyle { get; set; }
    public RectangleShapeProperties()
    {
    }
}

public class RectangleShape: DrawingShapeBase
{
    public RectangleShape() : base()
    {
        this.Properties = new RectangleShapeProperties();
    }

    public RectangleShape(string name, double x, double y, double width, double height) : base(name)
    {
        this.Shape = new Rectangle(x, y, width, height);
        this.Properties = new RectangleShapeProperties();
    }
    public new RectangleShapeProperties Properties { get; set; }
    public override string toSVG()
    {
        var s = this.Shape as Rectangle;

        string svg =
        "\t<rect id=\"" + this.Id + "\"" +
            " x=\"" + s.X + "\"" +
            " y=\"" + s.Y + "\"" +
            " width=\"" + s.Width + "\"" +
            " height=\"" + s.Height + "\"" +
            " style=\"" + Util.Utilility.GetSvgStyles(this.Style) + "\"/>\n";

        return svg;
    }
}
