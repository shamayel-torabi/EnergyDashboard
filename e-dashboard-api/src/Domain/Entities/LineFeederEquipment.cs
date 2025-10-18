
using Domain.Common;
using EnergyDashboard.Domain.Diagram.Shapes;
using EnergyDashboard.Domain.ValueObjects;

namespace EnergyDashboard.Domain.Entities;

public class LineFeederEquipment : Equipment
{
    private LineFeederEquipment(Guid id) : base(id)
    {
    }
    public Guid TransmissionLineId { get; private set; }
    public int LineFeederTypeId { get; private set; }
    public double? TransferCapacity { get; private set; }

    public Guid? SourceBusId { get;  set; }
    public Guid? DestinationBusId { get; set; }

    public void Update(LineFeederEquipment e)
    {
        CtId = e.CtId;
        BusbarId = e.BusbarId;
        BusbarName = e.BusbarName;
        CTRatio = e.CTRatio;
        PTRatio = e.PTRatio;
        Voltage = e.Voltage;
        Reverse = e.Reverse;
        AreaId = e.AreaId;
        ZoneId = e.ZoneId;
        SubstationId = e.SubstationId;

        Type = e.Type;
        Name = e.Name;
        IGMCToolId = e.IGMCToolId;
        DispatchingCode = e.DispatchingCode;
        IGMCCode = e.IGMCCode;

        _equipmentMeters.Clear();
        foreach (var em in e.EquipmentMeters)
        {
            _equipmentMeters.Add(em);
        }

        TransmissionLineId = e.TransmissionLineId;
        LineFeederTypeId = e.LineFeederTypeId;
        TransferCapacity = e.TransferCapacity;
    }

    public static LineFeederEquipment Create(string subId, BusBar bus, CT ct, LineFeeder linefeeder)
    {
        var id = new Guid(linefeeder.Id);
        Ensure.NotEmpty(id, "id cannot be null or empty", nameof(id));

        var ctId = new Guid(ct.Id);
        Ensure.NotEmpty(ctId, "CT.id cannot be null or empty", nameof(ctId));

        var busbarId = new Guid(bus.Id);

        var substationId = new Guid(subId);
        Ensure.NotEmpty(substationId, "substationId cannot be null or empty", nameof(substationId));


        var zoneId = new Guid(ct.ZoneId);
        Ensure.NotEmpty(zoneId, "ZoneId cannot be null or empty", nameof(zoneId));

        var areaId = new Guid(ct.AreaId);
        Ensure.NotEmpty(areaId, "AreaId cannot be null or empty", nameof(areaId));

        var equipment = new LineFeederEquipment(id);

        equipment.CtId = ctId;
        equipment.BusbarId = busbarId;
        equipment.BusbarName = bus.Properties.Name;
        equipment.PTRatio = Ratio.Create(ct.Properties.RatioPT);
        equipment.CTRatio = Ratio.Create(ct.Properties.RatioCT);
        equipment.Voltage = ct.Properties.Voltage;
        equipment.Reverse = ct.Properties.Reverse;
        equipment.SubstationId = substationId;
        equipment.ZoneId = zoneId;
        equipment.AreaId = areaId;

        foreach (var m in ct.Properties.Meters)
        {
            var equipmentmeter = EquipmentMeter.Create(equipment.Id, m);
            equipment._equipmentMeters.Add(equipmentmeter);
        }

        equipment.Type = linefeeder.GetType().Name;
        equipment.Name = linefeeder.Properties.Name;
        equipment.IGMCToolId = linefeeder.Properties.ToolId;
        equipment.DispatchingCode = linefeeder.Properties.DispachingCode;
        equipment.IGMCCode = linefeeder.Properties.IgmcCode;

        equipment.TransmissionLineId = new Guid(linefeeder.ConnectShapeId);
        equipment.LineFeederTypeId = linefeeder.Properties.LineFeederType;
        equipment.TransferCapacity = linefeeder.Properties.TransferCapacity;

        return equipment;
    }
}
