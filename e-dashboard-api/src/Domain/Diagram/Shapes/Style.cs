
namespace EnergyDashboard.Domain.Diagram.Shapes;

public class Style
{
    public Style()
    {
        this.Opacity = 1.0;
        this.FillStyle = "#00000000";
        this.StrokeStyle = "#000000";
        this.StrokeWidth = 1;
    }
    public double Opacity { get; set; }
    public int StrokeWidth { get; set; }
    public string StrokeStyle { get; set; }
    public string FillStyle { get; set; }
}
