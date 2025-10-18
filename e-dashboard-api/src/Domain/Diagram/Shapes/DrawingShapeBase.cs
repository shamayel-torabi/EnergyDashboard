using System.Text.Json.Serialization;

namespace EnergyDashboard.Domain.Diagram.Shapes;

public  interface IProperties
{
    string Name { get; set; }
    double Voltage { get; set; }
}
public class DrawingShapeBaseProperties : IProperties
{
    public string Name { get; set; }
    public double Voltage { get; set; }

    public DrawingShapeBaseProperties()
    {
    }
}


[JsonDerivedType(typeof(RectangleShape), "RectangleShape")]
[JsonDerivedType(typeof(CircleShape), "CircleShape")]
[JsonDerivedType(typeof(LineShape), "LineShape")]
[JsonDerivedType(typeof(PolyLineShape), "PolyLineShape")]
[JsonDerivedType(typeof(ConnectShape), "ConnectShape")]
[JsonDerivedType(typeof(BusBar), "BusBar")]
[JsonDerivedType(typeof(Breaker), "Breaker")]
[JsonDerivedType(typeof(CT), "CT")]
[JsonDerivedType(typeof(Transformer), "Transformer")]
[JsonDerivedType(typeof(LoadFeeder), "LoadFeeder")]
[JsonDerivedType(typeof(LineFeeder), "LineFeeder")]
[JsonDerivedType(typeof(Generator), "Generator")]
[JsonDerivedType(typeof(TransmisionLine), "TransmisionLine")]
[JsonDerivedType(typeof(BoxedConnectShape), "BoxedConnectShape")]
//[JsonPolymorphic(TypeDiscriminatorPropertyName = "typeName")]
public class DrawingShapeBase
{
    public DrawingShapeBase()
    {
        this.Id = Guid.NewGuid().ToString();
        this.Properties = new DrawingShapeBaseProperties();
        this.Shape = null;
        this.Ports = new List<PortShape>();
        this.Texts = new List<TextShape>();
        this.IsSelected = false;
        this.SelectionZoneWidth = 3;
        this.Style = new Style();
    }

    public DrawingShapeBase(string name) : this()
    {
        this.Properties.Name = name;
    }

    public string Id { get; set; }
    public string SubstationId { get; set; }
    public string ZoneId { get; set; }
    public string AreaId { get; set; }
    public DrawingShapeBaseProperties Properties { get; set; }

    public Shape Shape { get; set; }
    public IList<PortShape> Ports { get; set; }
    public IList<TextShape> Texts { get; set; }
    public bool IsSelected { get; set; }
    public int SelectionZoneWidth { get; set; }
    public Style Style { get; set; }
    public bool Changed { get; set; }

    [JsonIgnore]
    public double P
    {
        set
        {
            var v = value.ToString("F3");
            this.SetText("P", $"P={v} Mw");
        }
    }

    [JsonIgnore]
    public double Q
    {
        set
        {
            var v = value.ToString("F3");
            this.SetText("Q", $"Q={v} MVar");
        }
    }

    [JsonIgnore]
    public double Voltage
    {
        get
        {
            return this.Properties.Voltage;
        }
        set
        {
            this.Properties.Voltage = value;
            var v = value.ToString("F3");
            this.SetText("Voltage", $"V={v} Kv");
        }
    }

    [JsonIgnore]
    public string Name
    {
        get
        {
            return this.Properties.Name;
        }
        set
        {
            this.Properties.Name = value;
            this.SetText("Name", value);
        }
    }

    public void AddPort(string name, Point location, Point offset)
    {
        var port = new PortShape(name, offset);
        port.Location = location;
        this.Ports.Add(port);
        return;
    }

    public bool RemovePort(PortShape port)
    {
        return this.Ports.Remove(port);
    }

    public void AddText(string id, string text, Point location, Point offset, TextStyle style = null)
    {
        var t = new TextShape(id, text, offset);
        if (style != null)
            t.Style = style;

        t.Location = location;
        this.Texts.Add(t);
    }

    public void SetText(string id, string text)
    {
        var t = this.Texts.FirstOrDefault(x => x.Id == id);
        if(t != null)
            t.Text = text;
    }

    public virtual string toSVG()
    {
        return "";
    }
}
