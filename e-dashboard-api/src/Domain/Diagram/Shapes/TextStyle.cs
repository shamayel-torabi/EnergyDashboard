
namespace EnergyDashboard.Domain.Diagram.Shapes;

public class TextStyle : Style
{
    public TextStyle()
    {
        this.Opacity = 1;
        this.StrokeWidth = 1;
        this.StrokeStyle = "#000000";
        this.FillStyle = "#101010";
        this.FontFamily = "Vazir FD";
        this.FontSize = 8;
        this.FontStyle = "normal"; //oblique normal, italic, or bold
        this.TextAlign = "end"; // start, end, left, right, center
        this.TextBaseline = "middle"; //top hanging middle alphabetic ideographic bottom
        this.TextAnchor = "middle";
    }
    public string FontFamily { get; set; }
    public int FontSize { get; set; }
    public string FontStyle { get; set; }
    public string TextAlign { get; set; }
    public string TextBaseline { get; set; }
    public string TextAnchor { get; set; }
}
