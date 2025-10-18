
using System.Text.Json.Serialization;

namespace EnergyDashboard.Domain.Diagram.Shapes;

[JsonDerivedType(typeof(TextShape), nameof(TextShape))]
public class TextShape
{
    public TextShape()
    {
        this.Location = new Point(0, 0);
        this.IsVisible = true;
        this.Style = new TextStyle();
        this.Angle = 0.0;
    }
    public TextShape(string id, string text, Point offset) : this()
    {
        this.Id = id;
        this.Text = text;
        this.Offset = offset;
    }


    public string Id { get; set; }
    public string Text { get; set; }
    public Point Offset { get; set; }
    public Point Location { get; set; }
    public bool IsVisible { get; set; }
    public TextStyle Style { get; set; }
    public double Angle { get; set; }

    public string toSVG()
    {
        var x = this.Location.X + this.Offset.X;
        var y = this.Location.Y + this.Offset.Y;

        string svg =
            "\t\t<text id=\"" + this.Id + "\" " +
                    "x=\"" + x + "\" " +
                    "y=\"" + y + "\" " +
                    "font-family=\"" + this.Style.FontFamily + "\" " +
                    "font-size=\"" + this.Style.FontSize + "\" " +
                    "font-style=\"" + this.Style.FontStyle + "\" " +
                    "alignment-baseline=\"" + this.Style.TextBaseline + "\" " +
                    "text-anchor=\"" + this.Style.TextAnchor + "\" " +
                    "style=\"" + Util.Utilility.GetSvgStyles(this.Style) + "\"" + " >\n" +
                    "\t\t\t" + this.Text + "\n" +
                    "\t\t</text>\n";

        return svg;
    }
}
