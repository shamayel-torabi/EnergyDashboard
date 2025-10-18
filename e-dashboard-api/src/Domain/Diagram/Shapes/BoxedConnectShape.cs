using System.Text;

namespace EnergyDashboard.Domain.Diagram.Shapes;

public interface IBoxedConnectShapeProperties : IProperties
{
    int Width { get; set; }
    int Height { get; set; }
    string Description { get; set; }
    bool Abroad { get; set; }
    int IgmcStationId { get; set; }
}

public class BoxedConnectShapeProperties : IBoxedConnectShapeProperties
{
    public string Name { get; set; }
    public double Voltage { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public string Description { get; set; }
    public bool Abroad { get; set; }
    public int IgmcStationId { get; set; }

    public BoxedConnectShapeProperties()
    {
        this.Description = "شرح";
        this.Width = 200;
        this.Height = 30;
        this.Abroad = false;
        this.IgmcStationId = 0;
    }
}

public class BoxedConnectShape : BoxedShape
{
    public BoxedConnectShape() : base()
    {
        this.Properties = new BoxedConnectShapeProperties();
    }
    public BoxedConnectShape(string name, string mode, double x, double y) : base(name, x, y)
    {
        this.Properties = new BoxedConnectShapeProperties();
        this.Mode = mode;
    }

    public string Mode { get; set; }

    public new BoxedConnectShapeProperties Properties { get; set; }

    public override string toSVG()
    {
        StringBuilder markup = new StringBuilder();
        var s = this.Shape as Rectangle;

        var x = s.X + s.Width / 2;
        var y = s.Y + s.Height / 2;

        markup.AppendLine($"\t<g id=\"{this.Id}\"  transform=\"rotate({this.Shape.Angle * 90},{x},{y})\">\n");
        markup.AppendLine($"\t\t<rect id=\"{this.Id}\" x=\"{s.X}\" y=\"{s.Y}\" width=\"{s.Width}\" height=\"{s.Height}\" style=\"{Util.Utilility.GetSvgStyles(this.Style)}\"/>");

        if (this.Texts != null && this.Texts.Count > 0)
            foreach (var t in this.Texts)
                markup.AppendLine(t.toSVG());
        markup.AppendLine("\t</g>");

        return markup.ToString();
    }
}
