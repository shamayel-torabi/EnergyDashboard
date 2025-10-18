using EnergyDashboard.Domain.Diagram;
using EnergyDashboard.Domain.Entities;
using EnergyDashboard.Domain.Repository;
using EnergyDashboard.Domain.Diagram.Shapes;
using Application.Common.Exceptions;
using Microsoft.EntityFrameworkCore;
using EnergyDashboard.Infrastructure.Persistence;
using Newtonsoft.Json;

namespace EnergyDashboard.Infrastructure.Repository;

public class NetworkRepository : Repository<Guid, Network>, INetworkRepository
{
    public NetworkRepository(AppDbContext context):base(context)
    {
    }


    public async Task<IEnumerable<Network>> GetAllNetwork(CancellationToken cancellationToken = default)
    {
        return await _context.Networks
            .AsSplitQuery()
            .Include(n => n.Areas)
            .ThenInclude(a => a.Zones)
            .ThenInclude(z => z.Substations)
            .ToListAsync(cancellationToken);
    }

    public async Task<Network> Export(Guid id, CancellationToken cancellationToken = default)
    {
        var network = await _context.Networks
            .AsSplitQuery()
            .Include(n => n.Areas)
            .ThenInclude(a => a.Zones)
            .ThenInclude(z => z.Substations)
            .ThenInclude(e => e.Equipments)
            .SingleOrDefaultAsync(s => s.Id == id);

        return network;
    }

    public async Task Import(Network network, CancellationToken cancellationToken = default)
    {
        bool n = await _context.Networks.AnyAsync(a => a.Id == network.Id);

        if (n)
            _context.Networks.Update(network);
        else
            _context.Networks.Add(network);
    }

    public async Task UpdateDiagram(Guid id, DiagramModel networkDiagram, CancellationToken cancellationToken = default)
    {
        Network network = await _context.Networks.FindAsync(new object[] { id }, cancellationToken);

        if (network is null)
            throw new NotFoundException(nameof(Area), id);
        network.Update(networkDiagram);
        await UpdateAreas(networkDiagram);
        Update(network);
    }

    private async Task UpdateAreas(DiagramModel network)
    {
        var areaBoxes = network.GetBoxedConnectShapes();

        foreach (var a in _context.Areas)
        {
            if (!areaBoxes.Any(bx => bx.Id == a.Id.ToString()))
                DeleteArea(a);
        }

        foreach (var areaBox in areaBoxes)
        {
            var areaId = new Guid(areaBox.Id);
            Area area = await _context.Areas.FirstOrDefaultAsync(n => n.Id == areaId);
            DiagramModel areaDiagram;

            if (area is null)
            {
                area = new Area(areaId, new Guid(network.Id));
                //area.Name = areaBox.Properties.Name;

                areaDiagram = new DiagramModel(areaBox.Id, network.Id, "area");
                areaDiagram.Properties.Title = areaBox.Properties.Name;
                areaDiagram.Properties.Description = areaBox.Properties.Description;

                int num = 1;
                foreach (var port in areaBox.Ports)
                {
                    if (port.IsConnected && !string.IsNullOrEmpty(port.ConnectShapeId))
                    {
                        var tline = network.FindShape(port.ConnectShapeId) as TransmisionLine;
                        var name = tline?.Properties.Name;
                        var lineFeeder = new LineFeeder(name, num * 100, 80);
                        lineFeeder.AreaId = area.Id.ToString();
                        lineFeeder.Properties.LineFeederType = tline.Properties.LineFeederType;
                        lineFeeder.Properties.DispachingCode = tline.Properties.DispachingCode;
                        lineFeeder.Properties.IgmcCode = tline.Properties.IgmcCode;
                        lineFeeder.Properties.ToolId = tline.Properties.ToolId;
                        lineFeeder.Properties.TransferCapacity = tline.Properties.TransferCapacity;
                        lineFeeder.ConnectPortId = port.Id;
                        lineFeeder.ConnectShapeId = tline.Id;
                        areaDiagram.AddShape(lineFeeder);
                        num++;
                    }
                }

                //area.Diagram = JsonConvert.SerializeObject(areaDiagram);

                area.Update(areaDiagram);
                await _context.Areas.AddAsync(area);
            }
            else
            {
                //area.Name = areaBox.Properties.Name;

                areaDiagram = area.Diagram;

                areaDiagram.Properties.Title = areaBox.Properties.Name;
                areaDiagram.Properties.Description = areaBox.Properties.Description;

                var lineFeeders = areaDiagram.GetLineFeeders();
                List<LineFeeder> newLineFeeders = new List<LineFeeder>();

                foreach (var lf in lineFeeders)
                {
                    if (areaBox.Ports.Any(port => port.Id == lf.ConnectPortId))
                        newLineFeeders.Add(lf);
                    else
                    {
                        Tuple<string, string> tuple = areaDiagram.DeleteLineFeeder(lf);

                        if (!string.IsNullOrEmpty(tuple.Item1) && !string.IsNullOrEmpty(tuple.Item2))
                            tuple = await DeleteZoneLineFeeders(tuple.Item1, tuple.Item2);

                        if (!string.IsNullOrEmpty(tuple.Item1) && !string.IsNullOrEmpty(tuple.Item2))
                            await DeleteSubstationLineFeeders(tuple.Item1, tuple.Item2);
                    }
                }


                int num = lineFeeders.Count() + 1;

                foreach (var port in areaBox.Ports)
                {
                    if (port.IsConnected && !string.IsNullOrEmpty(port.ConnectShapeId))
                    {
                        LineFeeder lineFeeder = null;

                        if (newLineFeeders.Any(x => x.ConnectPortId == port.Id))
                        {
                            lineFeeder = newLineFeeders.SingleOrDefault(x => x.ConnectPortId == port.Id);
                            var tline = network.FindShape(port.ConnectShapeId) as TransmisionLine;

                            lineFeeder.Properties.Name = tline.Properties.Name;
                            lineFeeder.Properties.LineFeederType = tline.Properties.LineFeederType;
                            lineFeeder.Properties.DispachingCode = tline.Properties.DispachingCode;
                            lineFeeder.Properties.IgmcCode = tline.Properties.IgmcCode;
                            lineFeeder.Properties.ToolId = tline.Properties.ToolId;
                            lineFeeder.Properties.TransferCapacity = tline.Properties.TransferCapacity;
                            lineFeeder.SetText("Name", tline.Properties.Name);
                            //lineFeeder.ConnectPortId = port.Id;

                            var connLine = areaDiagram.FindLineFeederConnectedShape(lineFeeder) as TransmisionLine;
                            if (connLine is not null)
                            {
                                connLine.Properties.Name = lineFeeder.Properties.Name;
                                connLine.Properties.LineFeederType = lineFeeder.Properties.LineFeederType;
                                connLine.Properties.DispachingCode = lineFeeder.Properties.DispachingCode;
                                connLine.Properties.IgmcCode = lineFeeder.Properties.IgmcCode;
                                connLine.Properties.ToolId = lineFeeder.Properties.ToolId;
                                connLine.Properties.TransferCapacity = lineFeeder.Properties.TransferCapacity;

                                var zoneTuple = areaDiagram.FindNextBoxShape(connLine, lineFeeder.Id);
                                await UpdateZoneLineFeeders(zoneTuple.Item1, zoneTuple.Item2, connLine);
                            }
                        }
                        else
                        {
                            var tline = network.FindShape(port.ConnectShapeId) as TransmisionLine;
                            var name = tline?.Properties.Name;
                            lineFeeder = new LineFeeder(name, num * 100, 80);
                            lineFeeder.AreaId = area.Id.ToString();
                            lineFeeder.Properties.LineFeederType = tline.Properties.LineFeederType;
                            lineFeeder.Properties.DispachingCode = tline.Properties.DispachingCode;
                            lineFeeder.Properties.IgmcCode = tline.Properties.IgmcCode;
                            lineFeeder.Properties.ToolId = tline.Properties.ToolId;
                            lineFeeder.Properties.TransferCapacity = tline.Properties.TransferCapacity;
                            lineFeeder.ConnectPortId = port.Id;
                            lineFeeder.ConnectShapeId = tline.Id;
                            areaDiagram.AddShape(lineFeeder);
                            num++;
                        }
                    }
                }

                //area.Diagram = JsonConvert.SerializeObject(areaDiagram);

                area.Update(areaDiagram);
                _context.Areas.Update(area);
            }
        }
    }

    private void DeleteArea(Area area)
    {
        DiagramModel areaDiagram = area.Diagram;
        var lineFeeders = areaDiagram.GetLineFeeders();

        foreach (var lf in lineFeeders)
        {
            areaDiagram.RemoveShape(lf);
        }

        _context.Areas.Remove(area);
    }

    private async Task UpdateZoneLineFeeders(string zoneId, string portId, TransmisionLine tline)
    {
        if (zoneId == string.Empty || portId == string.Empty)
        {
            throw new ArgumentNullException("Error ZoneId or portId is Empty");
        }

        var zoneIdGuid = new Guid(zoneId);
        Zone zone = await _context.Zones.SingleOrDefaultAsync(z => z.Id == zoneIdGuid);

        if (zone is not null)
        {
            DiagramModel zoneDiagram = zone.Diagram;
            LineFeeder lineFeeder = zoneDiagram.FindLineFeeder(portId);

            if (lineFeeder is not null)
            {
                lineFeeder.Properties.Name = tline.Properties.Name;
                lineFeeder.Properties.LineFeederType = tline.Properties.LineFeederType;
                lineFeeder.Properties.DispachingCode = tline.Properties.DispachingCode;
                lineFeeder.Properties.IgmcCode = tline.Properties.IgmcCode;
                lineFeeder.Properties.ToolId = tline.Properties.ToolId;
                lineFeeder.Properties.TransferCapacity = tline.Properties.TransferCapacity;
                lineFeeder.SetText("Name", tline.Properties.Name);

                var connLine = zoneDiagram.FindLineFeederConnectedShape(lineFeeder) as TransmisionLine;
                if (connLine is not null)
                {
                    connLine.Properties.Name = lineFeeder.Properties.Name;
                    connLine.Properties.LineFeederType = lineFeeder.Properties.LineFeederType;
                    connLine.Properties.DispachingCode = lineFeeder.Properties.DispachingCode;
                    connLine.Properties.IgmcCode = lineFeeder.Properties.IgmcCode;
                    connLine.Properties.ToolId = lineFeeder.Properties.ToolId;
                    connLine.Properties.TransferCapacity = lineFeeder.Properties.TransferCapacity;

                    var tuple = zoneDiagram.FindNextBoxShape(connLine, lineFeeder.Id);
                    await UpdateSubstationLineFeeders(tuple.Item1, tuple.Item2, connLine);
                }

                zone.Update(zoneDiagram);
                //zone.Diagram = JsonConvert.SerializeObject(zoneDiagram);

                _context.Zones.Update(zone);
            }
            return;
        }
        return;
    }

    private async Task UpdateSubstationLineFeeders(string substationId, string portId, TransmisionLine tline)
    {
        if (substationId == string.Empty || portId == string.Empty)
        {
            throw new ArgumentNullException("Error ZoneId or portId is Empty");
        }

        var substationIdGuid = new Guid(substationId);
        Substation substation = await _context.Substations.SingleOrDefaultAsync(z => z.Id == substationIdGuid);
        if (substation is not null)
        {
            DiagramModel substationDiagram = substation.Diagram;
            LineFeeder lineFeeder = substationDiagram.FindLineFeeder(portId);

            if (lineFeeder is not null)
            {
                lineFeeder.Properties.Name = tline.Properties.Name;
                lineFeeder.Properties.LineFeederType = tline.Properties.LineFeederType;
                lineFeeder.Properties.DispachingCode = tline.Properties.DispachingCode;
                lineFeeder.Properties.IgmcCode = tline.Properties.IgmcCode;
                lineFeeder.Properties.ToolId = tline.Properties.ToolId;
                lineFeeder.Properties.TransferCapacity = tline.Properties.TransferCapacity;
                lineFeeder.SetText("Name", tline.Properties.Name);

                substation.Update(substationDiagram);
                _context.Substations.Update(substation);
            }
        }
    }
    private async Task<Tuple<string, string>> DeleteZoneLineFeeders(string zoneId, string portId)
    {
        if (zoneId == string.Empty || portId == string.Empty)
        {
            throw new ArgumentNullException("Error ZoneId or portId is Empty");
        }

        var zoneIdGuid = new Guid(zoneId);
        Zone zone = await _context.Zones.SingleOrDefaultAsync(z => z.Id == zoneIdGuid);
        if (zone is not null)
        {
            DiagramModel zoneDiagram = zone.Diagram;
            LineFeeder lf = zoneDiagram.FindLineFeeder(portId);
            Tuple<string, string> zp = zoneDiagram.DeleteLineFeeder(lf);

            zone.Update(zoneDiagram);
            //zone.Diagram = JsonConvert.SerializeObject(zoneDiagram);

            _context.Zones.Update(zone);
            return zp;
        }
        return Tuple.Create(string.Empty, string.Empty);
    }

    private async Task DeleteSubstationLineFeeders(string substationId, string portId)
    {
        if (substationId == string.Empty || portId == string.Empty)
        {
            throw new ArgumentNullException("Error ZoneId or portId is Empty");
        }

        var substationIdGuid = new Guid(substationId);
        Substation substation = await _context.Substations.SingleOrDefaultAsync(z => z.Id == substationIdGuid);
        if (substation is not null)
        {
            DiagramModel substationDiagram = substation.Diagram;
            LineFeeder lf = substationDiagram.FindLineFeeder(portId);
            substationDiagram.RemoveShape(lf);

            substation.Update(substationDiagram);
            _context.Substations.Update(substation);
        }
    }
}
