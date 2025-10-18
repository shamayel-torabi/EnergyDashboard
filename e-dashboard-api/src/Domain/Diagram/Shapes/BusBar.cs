using System.Text;
using System.Text.Json.Serialization;

namespace EnergyDashboard.Domain.Diagram.Shapes;

public interface IBusBarProperties : IProperties
{
    double VoltageMag { get; set; }
    double VoltageAng { get; set; }
    double PowerBalance { get; set; }
    string Color { get; set; }
    string DispachingCode { get; set; }
    string IgmcCode { get; set; }
}

public class BusBarProperties : IBusBarProperties
{
    public string Name { get; set; }
    public double Voltage { get; set; }
    public double VoltageMag { get; set; }
    public double VoltageAng { get; set; }
    public double PowerBalance { get; set; }
    public string Color { get; set; }
    public string DispachingCode { get; set; }
    public string IgmcCode { get; set; }
    public BusBarProperties()
    {
    }
}

public class BusBar : DrawingShapeBase
{
    public BusBar() : base()
    {
        this.Properties = new BusBarProperties();
    }

    public BusBar(string name) : base(name)
    {
        this.Properties = new BusBarProperties();
    }

    [JsonIgnore]
    public double Loss
    {
        set
        {
            var v = (value * 1000).ToString("F0");
            this.SetText("Loss", $"Loss={v} Kw");
        }
    }

    public new BusBarProperties Properties { get; set; }

    public override string toSVG()
    {
        this.Style.StrokeStyle = this.Properties.Color;
        StringBuilder markup = new StringBuilder();
        var s = this.Shape as Rectangle;

        markup.AppendLine($"\t<g id=\"{ this.Id }\" data-type=\"BusBar\">\n");

        markup.AppendLine(
        "\t\t<rect id=\"" + this.Id + "\"" +
            " x=\"" + s.X + "\"" +
            " y=\"" + s.Y + "\"" +
            " width=\"" + s.Width + "\"" +
            " height=\"" + s.Height + "\"" +
            " style=\"" + Util.Utilility.GetSvgStyles(this.Style) + "\"/>\n");

        if (this.Texts != null && this.Texts.Count != 0)
            foreach (var t in this.Texts)
                markup.AppendLine(t.toSVG());


        markup.AppendLine("\t</g>");

        return markup.ToString();
    }
}
