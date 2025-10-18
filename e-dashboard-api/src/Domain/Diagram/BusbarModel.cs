using EnergyDashboard.Domain.Entities;

namespace EnergyDashboard.Domain.Diagram;

public class BusbarModel
{
    public BusbarModel()
    {
        Equipments = new List<Equipment>();
    }

    public Guid BusbarId { get; set; }

    public string Name { get; set; }

    public double Voltage { get; set; }

    public Guid SubstationDiagramId { get; set; }

    public IList<Equipment> Equipments { get; set; }
}
