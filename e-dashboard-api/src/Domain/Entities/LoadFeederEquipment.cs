
using Domain.Common;
using EnergyDashboard.Domain.Diagram.Shapes;
using EnergyDashboard.Domain.ValueObjects;

namespace EnergyDashboard.Domain.Entities;

public class LoadFeederEquipment : Equipment
{
    private LoadFeederEquipment(Guid id) : base(id)
    {
    }

    public int LoadFeederTypeId { get; private set; }
    public virtual LoadFeederType LoadFeederType { get; private set;}

    public double? MaxDemand { get; private set; }

    public void Update(LoadFeederEquipment e)
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

        LoadFeederTypeId = e.LoadFeederTypeId;
        MaxDemand = e.MaxDemand;
    }

    public static LoadFeederEquipment Create(string subId, BusBar bus, CT ct, LoadFeeder loadfeeder)
    {
        var id = new Guid(loadfeeder.Id);
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

        var equipment = new LoadFeederEquipment(id);

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

        equipment.Type = loadfeeder.GetType().Name;
        equipment.Name = loadfeeder.Properties.Name;
        equipment.IGMCToolId = loadfeeder.Properties.ToolId;
        equipment.DispatchingCode = loadfeeder.Properties.DispachingCode;
        equipment.IGMCCode = loadfeeder.Properties.IgmcCode;

        equipment.LoadFeederTypeId = loadfeeder.Properties.LoadFeederSubType;
        equipment.MaxDemand = loadfeeder.Properties.MaxDemand;

        return equipment;
    }
}
