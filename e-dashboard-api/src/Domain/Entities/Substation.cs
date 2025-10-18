using Domain.Common;
using EnergyDashboard.Domain.Diagram;

namespace EnergyDashboard.Domain.Entities;

public class Substation : Entity<Guid>
{
    public Substation(Guid id, Guid zoneId) : base(id)
    {
        Ensure.NotEmpty(id, "id cannot be empty", nameof(id));
        Ensure.NotEmpty(zoneId, "The zoneId is required.", nameof(zoneId));

        Type = "Substation";
        Attributes = "{\"data-type\":\"diagram\"}";
        ZoneId = zoneId;
    }
    public string Attributes { get;}
    public string Name { get; set; }

    public string Type { get; private set; }
    public DiagramModel Diagram { get; private set; }
    public int IGMCStationId { get; private set; } = 0;
    public string IGMCCode { get; private set; }
    public string DispatchingCode { get; private set; }
    public DateTimeOffset? MountDate { get; private set; }
    public DateTimeOffset? DismountDate { get; private set; }
    public Guid ZoneId { get; private set; }


    protected readonly List<Equipment> _equipments = new();

    public IReadOnlyCollection<Equipment> Equipments => _equipments.AsReadOnly();

    public override string ToString()
    {
        return $"Id:{Id}, Name:{Name}";
    }

    public void Update(DiagramModel substationDiagram)
    {
        Ensure.NotNull(substationDiagram, "substationDiagram cannot be null", nameof(substationDiagram));
        Ensure.NotEmpty(substationDiagram.Properties.Title, "Substation name cannot be Null", nameof(substationDiagram.Properties.Title));

        Name = substationDiagram.Properties.Title;
        Type = substationDiagram.GetType().Name;
        IGMCStationId = substationDiagram.Properties.IgmcStationId;
        IGMCCode = substationDiagram.Properties.IgmcCode;
        MountDate = substationDiagram.Properties.MountDate;
        DismountDate = substationDiagram.Properties.DismountDate;
        DispatchingCode = substationDiagram.Properties.DispachingCode;
        Diagram = substationDiagram;
    }

    //public void UpdateEquipmens(DiagramModel substationDiagram)
    //{
    //    var equipments = substationDiagram.GetEquipments();
    //    Guid diagramId = new Guid(substationDiagram.Id);

    //    if(equipments is null)
    //    {
    //        _equipments.Clear();
    //        return;
    //    }

    //    foreach (var eq in Equipments)
    //    {
    //        if (!equipments.Any(x => x.Id == eq.Id))
    //        {
    //            _equipments.Remove(eq);
    //        }
    //    }

    //    foreach (var e in equipments)
    //    {
    //        if (Equipments.Any(x => x.Id == e.Id))
    //        {
    //            var equipment = Equipments.SingleOrDefault(x => x.Id == e.Id);

    //            switch (e.Type)
    //            {
    //                case "Generator":
    //                    var ppf = Equipments.SingleOrDefault(x => x.Id == e.Id) as PowerplantFeederEquipment;

    //                    if (ppf is not null)
    //                    {
    //                        ppf.Update(e as PowerplantFeederEquipment);
    //                    }
    //                    break;
    //                case "Transformer":
    //                    var tff = Equipments.SingleOrDefault(x => x.Id == e.Id) as TransformerFeederEquipment;

    //                    if (tff is not null)
    //                    {
    //                        tff.Update(e as TransformerFeederEquipment);
    //                    }
    //                    break;
    //                case "LoadFeeder":
    //                    var lof = Equipments.SingleOrDefault(x => x.Id == e.Id) as LoadFeederEquipment;

    //                    if (lof is not null)
    //                    {
    //                        lof.Update(e as LoadFeederEquipment);
    //                    }
    //                    break;
    //                case "LineFeeder":
    //                    var lif = Equipments.SingleOrDefault(x => x.Id == e.Id) as LineFeederEquipment;

    //                    if (lif is not null)
    //                    {
    //                        lif.Update(e as LineFeederEquipment);
    //                    }
    //                    break;
    //                default:
    //                    throw new DomainException(DomainErrors.DiagramModel.BadEquipmentType);
    //            }
    //        }
    //        else
    //        {
    //            switch (e.Type)
    //            {
    //                case "Generator":
    //                    _equipments.Add(e as PowerplantFeederEquipment);
    //                    break;
    //                case "Transformer":
    //                    _equipments.Add(e as TransformerFeederEquipment);
    //                    break;
    //                case "LoadFeeder":
    //                    _equipments.Add(e as LoadFeederEquipment);
    //                    break;
    //                case "LineFeeder":
    //                    _equipments.Add(e as LineFeederEquipment);
    //                    break;
    //                default:
    //                    throw new DomainException(DomainErrors.DiagramModel.BadEquipmentType);
    //            }
    //        }
    //    }
    //}
}
