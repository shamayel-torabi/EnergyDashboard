

namespace EnergyDashboard.Domain.Diagram.Shapes;

public class LineConnectShape: DrawingShapeBase
{
    public LineConnectShape() : base()
    {
        this.SrcShapeId = string.Empty;
        this.SrcPortName = string.Empty;
        this.DesShapeId = string.Empty;
        this.DesPortName = string.Empty;
        this.ConnectShapeType = string.Empty;


    }
    public LineConnectShape(string name) : base(name)
    {
        this.SrcShapeId = string.Empty;
        this.SrcPortName = string.Empty;
        this.DesShapeId = string.Empty;
        this.DesPortName = string.Empty;
        this.ConnectShapeType = string.Empty;
    }

    public string SrcShapeId { get; set; }
    public string DesShapeId { get; set; }
    public string SrcPortName { get; set; }
    public string DesPortName { get; set; }
    public string ConnectShapeType { get; set; }
}
