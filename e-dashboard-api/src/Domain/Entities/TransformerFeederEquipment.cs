using Domain.Common;
using EnergyDashboard.Domain.Diagram.Shapes;
using EnergyDashboard.Domain.Enums;
using EnergyDashboard.Domain.ValueObjects;

namespace EnergyDashboard.Domain.Entities;

public class TransformerFeederEquipment: Equipment
{
    private TransformerFeederEquipment(Guid id) : base(id)
    {
    }
    public TransofmerType TransofmerType { get; private set; }

    public double? MVA { get; private set; }

    public double? PrimaryVoltage { get; private set; }
    public double? SecondaryVoltage { get; private set; }

    public Guid? SourceBusId { get; set; }
    public Guid? DestinationBusId { get; set; }

    public void Update(TransformerFeederEquipment e)
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

        TransofmerType = e.TransofmerType;
        MVA = e.MVA;
        PrimaryVoltage = e.PrimaryVoltage;
        SecondaryVoltage = e.SecondaryVoltage;
    }

    public static TransformerFeederEquipment Create(string subId, BusBar bus, CT ct, Transformer transformer)
    {
        var id = new Guid(transformer.Id);
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

        var equipment = new TransformerFeederEquipment(id);

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

        equipment.Type = transformer.GetType().Name;
        equipment.Name = transformer.Properties.Name;
        equipment.IGMCToolId = transformer.Properties.ToolId;
        equipment.DispatchingCode = transformer.Properties.DispachingCode;
        equipment.IGMCCode = transformer.Properties.IgmcCode;

        equipment.TransofmerType = transformer.Properties.TransofmerType;
        equipment.MVA = transformer.Properties.Mva;
        equipment.PrimaryVoltage = transformer.Properties.PrimaryVoltage;
        equipment.SecondaryVoltage = transformer.Properties.SecondaryVoltage;

        return equipment;
    }
}
