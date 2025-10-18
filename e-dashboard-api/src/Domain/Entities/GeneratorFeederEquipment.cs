using Domain.Common;
using EnergyDashboard.Domain.Diagram.Shapes;
using EnergyDashboard.Domain.Enums;
using EnergyDashboard.Domain.ValueObjects;

namespace EnergyDashboard.Domain.Entities;

public class GeneratorFeederEquipment: Equipment
{
    private GeneratorFeederEquipment(Guid id) : base(id)
    {
    }

    public int PowerplantTypeId { get; private set; }
    public virtual PowerplantType PowerplantType { get; private set; }

    public int PowerplantOperatorId { get; set; }
    public virtual PowerplantOperator PowerplantOperator { set; get; }

    public GenerationType PowerplantScale { get; private set; }
    public double? Capacity { get; private set; }
    public double? NominalCapacity { get; private set; }

    public void Update(GeneratorFeederEquipment e)
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
        foreach(var em in e.EquipmentMeters)
        {
            _equipmentMeters.Add(em);
        }

        PowerplantScale = e.PowerplantScale;
        PowerplantTypeId = e.PowerplantTypeId;
        PowerplantOperatorId = e.PowerplantOperatorId;
        Capacity = e.Capacity;
        NominalCapacity = e.NominalCapacity;
    }

    public static GeneratorFeederEquipment Create(string subId, BusBar bus, CT ct, Generator generator)
    {
        var id = new Guid(generator.Id);
        Ensure.NotEmpty(id, "generator.Id cannot be null or empty", nameof(id));

        var ctId = new Guid(ct.Id);
        Ensure.NotEmpty(ctId, "CT.id cannot be null or empty", nameof(ctId));

        var busbarId = new Guid(bus.Id);

        var substationId = new Guid(subId);
        Ensure.NotEmpty(substationId, "substation.Id cannot be null or empty", nameof(substationId));

        var zoneId = new Guid(ct.ZoneId);
        Ensure.NotEmpty(zoneId, "Zone.Id cannot be null or empty", nameof(zoneId));

        var areaId = new Guid(ct.AreaId);
        Ensure.NotEmpty(areaId, "Area.Id cannot be null or empty", nameof(areaId));

        var equipment = new GeneratorFeederEquipment(id);

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

        equipment.Type = generator.GetType().Name;
        equipment.Name = generator.Properties.Name;
        equipment.IGMCToolId = generator.Properties.ToolId;
        equipment.DispatchingCode = generator.Properties.DispachingCode;
        equipment.IGMCCode = generator.Properties.IgmcCode;

        if (generator.Properties.GenSize == 2)
            equipment.PowerplantScale = GenerationType.DG;
        else
            equipment.PowerplantScale = GenerationType.Conventional;

        equipment.PowerplantTypeId = generator.Properties.GenType;
        equipment.PowerplantOperatorId = generator.Properties.Operator;
        equipment.Capacity = generator.Properties.Capacity;
        equipment.NominalCapacity = generator.Properties.NominalCapacity;

        return equipment;
    }
}
