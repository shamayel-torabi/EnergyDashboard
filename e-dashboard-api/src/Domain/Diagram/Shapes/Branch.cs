
namespace EnergyDashboard.Domain.Diagram.Shapes;

public class Branch: List<DrawingShapeBase>
{
    public string SrcBus { get; set; }
    public string DestBus { get; set; }
}

public class Branches : Dictionary<string, Branch>
{

}
