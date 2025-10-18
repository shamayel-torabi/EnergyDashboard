using System.Text;
using System.Text.Json.Serialization;
using EnergyDashboard.Domain.Diagram.Shapes;
using EnergyDashboard.Domain.Entities;
using EnergyDashboard.Domain.Exceptions;

namespace EnergyDashboard.Domain.Diagram;


public class DiagramModelProperties
{
    public string Title { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public string BackgroundColor { get; set; }
    public string Description { get; set; }
    public DateTime MountDate { get; set; }
    public DateTime DismountDate { get; set; }
    public string DispachingCode { get; set; }
    public string IgmcCode { get; set; }
    public int IgmcStationId { get; set; }


    public DiagramModelProperties()
    {
        this.Title = "دیاگرام جدید";
        this.Width = 1200;
        this.Height = 800;
        this.BackgroundColor = "#FFFFFF";
        this.Description = "شرح دیاگرام";
        this.MountDate = DateTime.Now;
        this.DismountDate = DateTime.Now.AddYears(50);
        this.DispachingCode = "Code";
        this.IgmcCode = "IgmcCode";
        this.IgmcStationId = 0;
    }
}

[JsonDerivedType(typeof(DiagramModel), nameof(DiagramModel))]
public class DiagramModel : IEquatable<DiagramModel>
{
    public DiagramModel()
    {
        this.Shapes = new List<DrawingShapeBase>();
        this.Properties = new DiagramModelProperties();
        this.Changed = false;
    }
    public DiagramModel(string id, string parentId, string diagramMode) : this()
    {
        this.Id = id;
        this.ParentId = parentId;
        this.DiagramMode = diagramMode;
    }

    public string Id { get; set; }
    public string DiagramMode { get; set; }
    public string ParentId { get; set; }

    public IList<DrawingShapeBase> Shapes { get; set; }

    public DiagramModelProperties Properties { get; set; }

    public bool Changed { get; set; }

    public override bool Equals(object obj)
    {
        return Equals(obj as DiagramModel);
    }

    public bool Equals(DiagramModel other)
    {
        return other != null &&
               EqualityComparer<string>.Default.Equals(Id, other.Id);
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }

    public static bool operator ==(DiagramModel entity1, DiagramModel entity2)
    {
        return EqualityComparer<DiagramModel>.Default.Equals(entity1, entity2);
    }

    public static bool operator !=(DiagramModel entity1, DiagramModel entity2)
    {
        return !(entity1 == entity2);
    }

    public void AddShape(DrawingShapeBase shape)
    {
        this.Shapes.Add(shape);
    }

    public bool RemoveShape(DrawingShapeBase shape)
    {
        if (shape is LineConnectShape)
        {
            RemoveConnectedShape(shape as LineConnectShape);
        }
        else
        {
            if (shape.Ports.Count > 0)
            {
                foreach (var port in shape.Ports)
                {
                    if (port.IsConnected && !string.IsNullOrEmpty(port.ConnectShapeId))
                    {
                        var s = this.FindShape(port.ConnectShapeId);
                        if (s != null)
                        {
                            port.ConnectShapeId = string.Empty;
                            port.IsConnected = false;
                            RemoveShape(s);
                        }
                    }
                }
            }
        }

        return this.Shapes.Remove(shape);
    }

    public DrawingShapeBase FindShape(string id)
    {
        if (string.IsNullOrEmpty(id))
            return null;
        return Shapes.FirstOrDefault(s => s.Id == id);
    }

    public string toSVG(bool preAmble = false)
    {
        var svg = new StringBuilder();
        if (preAmble)
        {
            svg.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"no\" ?>");
            svg.AppendLine("<!DOCTYPE svg PUBLIC \" -//W3C//DTD SVG 1.1//EN\"");
            svg.AppendLine("\"http://www.w3.org/Graphics/SVG/1.1/DTD/svg11.dtd\" ?>");
        }

        svg.AppendFormat($"<svg Id=\"{this.Id}\" data-type=\"SubstationDiagramModel\" xmlns=\"http://www.w3.org/2000/svg\" xmlns:xlink=\"http://www.w3.org/1999/xlink\" version=\"1.1\" width=\"{this.Properties.Width}\" height=\"{this.Properties.Height}\" xml:space=\"preserve\" style=\"background: {this.Properties.BackgroundColor}\" viewBox=\"0 0 {this.Properties.Width} {this.Properties.Height}\">\n");
        svg.AppendLine("\t<desc>Created with EnergyDashboard App </desc>");
        foreach (var shape in Shapes)
        {
            svg.Append(shape.toSVG());
        }
        svg.AppendLine("</svg>");
        return svg.ToString();

    }


    public Tuple<string, string> FindNextBoxShape(TransmisionLine tline, string lineFeederId)
    {
        Tuple<string, string> ret = Tuple.Create<string, string>(string.Empty, string.Empty);
        string portId = string.Empty;
        string boxShapeId = string.Empty;
        DrawingShapeBase boxShape = null;

        if (tline.SrcShapeId == lineFeederId)
        {
            boxShape = FindShape(tline.DesShapeId);
            if (boxShape != null)
            {
                boxShapeId = boxShape.Id;
                var port = boxShape.Ports.FirstOrDefault(p => p.IsConnected && p.ConnectShapeId == tline.Id);
                portId = port != null ? port.Id : string.Empty;
            }

            ret = Tuple.Create<string, string>(boxShapeId, portId);
        }
        else if (tline.DesShapeId == lineFeederId)
        {
            boxShape = FindShape(tline.SrcShapeId);
            if (boxShape != null)
            {
                boxShapeId = boxShape.Id;
                var port = boxShape.Ports.FirstOrDefault(p => p.IsConnected && p.ConnectShapeId == tline.Id);
                portId = port != null ? port.Id : string.Empty;
            }
            ret = Tuple.Create<string, string>(boxShapeId, portId);
        }

        return ret;
    }

    public DrawingShapeBase FindLineFeederConnectedShape(LineFeeder lineFeeder)
    {
        PortShape port = lineFeeder.Ports[0];
        if (port != null && port.IsConnected && port.ConnectShapeId != string.Empty)
            return FindShape(port.ConnectShapeId);

        return null;
    }

    public LineFeeder FindLineFeeder(string portId)
    {
        IList<LineFeeder> lineFeeders = GetLineFeeders();
        LineFeeder lf = lineFeeders.FirstOrDefault(x => x.ConnectPortId == portId);
        return lf;
    }

    public IList<LineFeeder> GetLineFeeders()
    {
        List<LineFeeder> lineFeeders = new List<LineFeeder>();
        var shapes = Shapes.Where(w => w is LineFeeder);
        foreach (var shp in shapes)
            lineFeeders.Add(shp as LineFeeder);

        return lineFeeders;
    }

    public List<CT> BusBranchCT(BusBar bus)
    {
        bool branchEnd = false;
        List<CT> cts = new List<CT>();
        CT ct = null;

        foreach (var port in bus.Ports)
        {
            ct = null;
            branchEnd = false;
            DrawingShapeBase s = FindShape(port.ConnectShapeId);
            string bid = bus.Id;

            while (!branchEnd)
            {
                switch (s.GetType().Name)
                {
                    case "ConnectShape":
                        var connect = s as ConnectShape;
                        s = FindNextShape(connect, bid);
                        bid = connect.Id;
                        break;
                    case "Breaker":
                        var breaker = s as Breaker;
                        s = FindNextShape(breaker, bid);
                        bid = breaker.Id;
                        break;
                    case "CT":
                        ct = s as CT;
                        branchEnd = true;
                        break;
                    case "TransmisionLine":
                        break;
                    case "Transformer":
                    case "Generator":
                    case "LineFeeder":
                    case "LoadFeeder":
                    case "BusBar":
                        branchEnd = true;
                        break;
                    default:
                        throw new DomainException(DomainErrors.DiagramModel.BadNetworkConnection);
                }
            }

            if (ct != null)
                cts.Add(ct);
        }
        return cts;
    }

    public Tuple<string, string> DeleteLineFeeder(LineFeeder lf)
    {
        Tuple<string, string> ret = Tuple.Create(string.Empty, string.Empty);

        if (lf.Ports.Count > 0)
        {
            foreach (var port in lf.Ports.ToList())
            {
                if (port.IsConnected && !string.IsNullOrEmpty(port.ConnectShapeId))
                {
                    var lineShape = this.FindShape(port.ConnectShapeId) as LineConnectShape;

                    if (lineShape != null)
                    {
                        ret = RemoveConnectedShape(lineShape);
                        this.Shapes.Remove(lineShape);
                    }
                }
            }
        }

        this.Shapes.Remove(lf);
        return ret;
    }

    public IList<BusbarModel> GetBusbarModels()
    {
        IList<BusbarModel> busbarModels = new List<BusbarModel>();

        foreach (var shape in this.Shapes)
        {
            if (shape is BusBar)
            {
                var bus = shape as BusBar;
                var busbar = new BusbarModel();
                busbar.BusbarId = new Guid(bus.Id);
                busbar.SubstationDiagramId = new Guid(this.Id);
                busbar.Name = bus.Properties.Name;
                busbar.Voltage = bus.Properties.Voltage;
                busbar.Equipments = GetBusEquipment(bus);
                busbarModels.Add(busbar);
            }
        }

        return busbarModels;
    }

    public IEnumerable<Equipment> GetEquipments()
    {
        bool branchEnd = false;
        List<DrawingShapeBase> branch = new List<DrawingShapeBase>();
        Stack<string> link = new Stack<string>();
        IDictionary<string, List<DrawingShapeBase>> branches = new Dictionary<string, List<DrawingShapeBase>>();
        IList<Equipment> equipments = new List<Equipment>();

        foreach (var shape in Shapes)
        {
            if (shape is BusBar)
            {
                var bus = shape as BusBar;

                foreach (var port in bus.Ports)
                {
                    branch.Clear();
                    link.Clear();
                    branchEnd = false;
                    DrawingShapeBase s = FindShape(port.ConnectShapeId);
                    Equipment equipment = null;
                    CT ct = null;

                    string bid = bus.Id;

                    link.Push(bus.Id);
                    branch.Add(bus);

                    while (!branchEnd)
                    {
                        switch (s.GetType().Name)
                        {
                            case "ConnectShape":
                                var connect = s as ConnectShape;
                                s = FindNextShape(connect, bid);
                                bid = connect.Id;
                                break;
                            case "Breaker":
                                var breaker = s as Breaker;
                                s = FindNextShape(breaker, bid);
                                bid = breaker.Id;
                                break;
                            case "CT":
                                ct = s as CT;
                                link.Push(ct.Id);
                                branch.Add(ct);

                                s = FindNextShape(ct, bid);
                                bid = ct.Id;
                                break;
                            case "Transformer":
                                var transformer = s as Transformer;
                                link.Push(transformer.Id);
                                branch.Add(transformer);

                                if (ct != null)
                                    equipment = TransformerFeederEquipment.Create(this.Id, bus, ct, transformer);

                                branchEnd = true;
                                break;
                            case "Generator":
                                var generator = s as Generator;
                                link.Push(generator.Id);
                                branch.Add(generator);

                                if (ct != null)
                                    equipment = GeneratorFeederEquipment.Create(this.Id, bus, ct, generator);

                                branchEnd = true;
                                break;
                            case "LineFeeder":
                                var linefeeder = s as LineFeeder;
                                link.Push(linefeeder.Id);
                                branch.Add(linefeeder);

                                if (ct != null)
                                    equipment = LineFeederEquipment.Create(this.Id, bus, ct, linefeeder);

                                branchEnd = true;
                                break;
                            case "LoadFeeder":
                                var loadfeeder = s as LoadFeeder;
                                link.Push(loadfeeder.Id);
                                branch.Add(loadfeeder);

                                if (ct != null)
                                    equipment = LoadFeederEquipment.Create(this.Id, bus, ct, loadfeeder);

                                branchEnd = true;
                                break;

                            default:
                                throw new DomainException(DomainErrors.DiagramModel.BadNetworkConnection);
                        }
                    }

                    string key = StackToString(link.ToArray());
                    string revkey = StackToString(link.Reverse().ToArray());

                    if (!(branches.ContainsKey(key) || branches.ContainsKey(revkey)))
                    {
                        branches.Add(key, branch);

                        if (equipment != null)
                            equipments.Add(equipment);
                    }

                }
            }
        }

        return equipments;
    }

    public IEnumerable<BoxedConnectShape> GetBoxedConnectShapes()
    {
        IList<BoxedConnectShape> boxedConnectShape = new List<BoxedConnectShape>();

        foreach (var shape in this.Shapes)
        {
            if (shape is BoxedConnectShape)
            {
                var boxedConnect = shape as BoxedConnectShape;
                boxedConnectShape.Add(boxedConnect);
            }
        }

        return boxedConnectShape;
    }


    private IList<Equipment> GetBusEquipment(BusBar bus)
    {
        bool branchEnd = false;
        IList<Equipment> equipments = new List<Equipment>();

        foreach (var port in bus.Ports)
        {
            branchEnd = false;
            DrawingShapeBase s = FindShape(port.ConnectShapeId);
            Equipment equipment = null;
            CT ct = null;
            Transformer transformer = null;

            string bid = bus.Id;

            while (!branchEnd)
            {
                if (s == null)
                    break;

                switch (s.GetType().Name)
                {
                    case "ConnectShape":
                        var connect = s as ConnectShape;
                        s = FindNextShape(connect, bid);
                        bid = connect.Id;
                        break;
                    case "Breaker":
                        var breaker = s as Breaker;
                        s = FindNextShape(breaker, bid);
                        bid = breaker.Id;
                        break;
                    case "CT":
                        ct = s as CT;

                        if (transformer != null)
                            equipment = TransformerFeederEquipment.Create(this.Id, bus, ct, transformer);

                        s = FindNextShape(ct, bid);
                        bid = ct.Id;
                        break;
                    case "Transformer":
                        transformer = s as Transformer;

                        if (ct != null)
                            equipment = TransformerFeederEquipment.Create(this.Id, bus, ct, transformer);

                        s = FindNextShape(transformer, bid);
                        bid = transformer.Id;
                        break;
                    case "Generator":
                        var generator = s as Generator;

                        if (ct != null)
                            equipment = GeneratorFeederEquipment.Create(this.Id, bus, ct, generator);

                        branchEnd = true;
                        break;
                    case "LineFeeder":
                        var linefeeder = s as LineFeeder;

                        if (ct != null)
                            equipment = LineFeederEquipment.Create(this.Id, bus, ct, linefeeder);

                        branchEnd = true;
                        break;
                    case "LoadFeeder":
                        var loadfeeder = s as LoadFeeder;

                        if (ct != null)
                            equipment = LoadFeederEquipment.Create(this.Id, bus, ct, loadfeeder);

                        branchEnd = true;
                        break;
                    case "BusBar":
                        var busbar = s as BusBar;
                        branchEnd = true;
                        break;
                    default:
                        throw new DomainException(DomainErrors.DiagramModel.BadNetworkConnection);
                }
            }

            if (equipment != null)
                equipments.Add(equipment);
        }
        return equipments;
    }

    private string StackToString(string[] li)
    {
        StringBuilder sb = new StringBuilder();
        foreach (var s in li)
        {
            sb.AppendLine(s);
        }
        return sb.ToString();
    }

    private DrawingShapeBase FindNextShape(DrawingShapeBase shape, string id)
    {
        if (shape is ConnectShape)
        {
            var connect = shape as ConnectShape;
            if (connect.SrcShapeId == id)
                return FindShape(connect.DesShapeId);
            else if (connect.DesShapeId == id)
                return FindShape(connect.SrcShapeId);
            else
                return null;
        }
        else if (shape is TransmisionLine)
        {
            var connect = shape as TransmisionLine;
            if (connect.SrcShapeId == id)
                return FindShape(connect.DesShapeId);
            else if (connect.DesShapeId == id)
                return FindShape(connect.SrcShapeId);
            else
                return null;
        }
        else
        {
            string shapeId = string.Empty;
            foreach (var port in shape.Ports)
            {
                if (port.ConnectShapeId != id)
                    shapeId = port.ConnectShapeId;
            }
            return FindShape(shapeId);
        }
    }

    private Tuple<string, string> RemoveConnectedShape(LineConnectShape conncShape)
    {
        var ret = Tuple.Create(string.Empty, string.Empty);
        var shape = this.FindShape(conncShape.SrcShapeId);
        var ports = shape?.Ports.ToList();

        foreach (var port in ports)
        {
            if (port.ConnectShapeId == conncShape.Id)
            {
                if (shape is BusBar || shape is BoxedConnectShape)
                {
                    ret = Tuple.Create(shape.Id, port.Id);
                    shape.RemovePort(port);
                }
                else
                {
                    port.IsConnected = false;
                    port.ConnectShapeId = string.Empty;
                }

                Disconnect(shape);
            }
        }

        shape = this.FindShape(conncShape.DesShapeId);
        ports = shape?.Ports.ToList();

        foreach (var port in ports)
        {
            if (port.ConnectShapeId == conncShape.Id)
            {
                if (shape is BusBar || shape is BoxedConnectShape)
                {
                    ret = Tuple.Create(shape.Id, port.Id);
                    shape.RemovePort(port);
                }
                else
                {
                    port.IsConnected = false;
                    port.ConnectShapeId = string.Empty;
                }

                Disconnect(shape);
            }
        }
        return ret;
    }

    private void Disconnect(DrawingShapeBase shape)
    {
        if (shape.Ports.Count > 0)
        {
            foreach (var port in shape.Ports)
            {
                if (!(port.IsConnected && !string.IsNullOrEmpty(port.ConnectShapeId)))
                {
                    shape.Style.StrokeStyle = "#000000";
                    if (shape is Transformer)
                    {
                        var tshap = shape as Transformer;
                        if (port.Name == "1" && tshap.StrokeStylePrimary != "#000000")
                            tshap.StrokeStylePrimary = "#000000";
                        else if (port.Name == "2" && tshap.StrokeStyleSecondary != "#000000")
                            tshap.StrokeStyleSecondary = "#000000";
                    }
                }
            }
        }
    }
}
