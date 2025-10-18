using System.Text;

namespace EnergyDashboard.Domain.Diagram.Shapes;

public interface IConnectShapeProperties : IProperties
{
    string SrcShapeName { get; set; }
    string DesShapeName { get; set; }
}

public class ConnectShapeProperties : IConnectShapeProperties
{
    public string Name { get; set; }
    public double Voltage { get; set; }
    public string SrcShapeName { get; set; }
    public string DesShapeName { get; set; }
    public ConnectShapeProperties()
    {
        this.SrcShapeName = string.Empty;
        this.DesShapeName = string.Empty;
    }
}

public class ConnectShape : LineConnectShape
{
    public ConnectShape() : base()
    {
        this.Properties = new ConnectShapeProperties();
        this.Shape = new Line(new Point(0, 0), new Point(0, 0));
    }
    public ConnectShape(string name) : base(name)
    {
        this.Properties = new ConnectShapeProperties();
        this.Shape = new Line(new Point(0, 0), new Point(0, 0));
    }

    public new ConnectShapeProperties Properties { get; set; }

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
