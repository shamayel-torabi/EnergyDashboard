using EnergyDashboard.Domain.Diagram;
using EnergyDashboard.Domain.Entities;
using EnergyDashboard.Domain.Repository;
using Microsoft.EntityFrameworkCore;
using Application.Common.Exceptions;
using EnergyDashboard.Domain.Diagram.Shapes;
using EnergyDashboard.Infrastructure.Persistence;
using Newtonsoft.Json;

namespace EnergyDashboard.Infrastructure.Repository;

public class AreaRepository : Repository<Guid, Area>, IAreaRepository
{

    public AreaRepository(AppDbContext context):base(context)
    {
    }

    public async Task UpdateDiagram(Guid id, DiagramModel areaDiagram, CancellationToken cancellationToken = default)
    {
        Area area = await _context.Areas.FindAsync(new object[] { id }, cancellationToken);

        if (area is null)
            throw new NotFoundException(nameof(Area), id);

        area.Update(areaDiagram);
        await UpdateZones(areaDiagram, cancellationToken);

        Update(area);
    }

    private async Task UpdateZones(DiagramModel areaDiagram, CancellationToken cancellationToken)
    {
        var zoneBoxes = areaDiagram.GetBoxedConnectShapes();
        var areaZones = await _context.Zones.Where(z => z.AreaId.ToString() == areaDiagram.Id).ToListAsync(cancellationToken);

        foreach (var z in areaZones)
        {
            if (!zoneBoxes.Any(bx => bx.Id == z.Id.ToString()))
                DeleteZone(z);
        }

        foreach (var zoneBox in zoneBoxes)
        {
            var zoneId = new Guid(zoneBox.Id);
            Zone zone = await _context.Zones.FirstOrDefaultAsync(n => n.Id == zoneId, cancellationToken);

            if (zone is null)
            {
                zone = new Zone(zoneId, new Guid(areaDiagram.Id));
                //zone.Name = zoneBox.Properties.Name;

                var zoneDiagram = new DiagramModel(zoneBox.Id, areaDiagram.Id, "zone");
                zoneDiagram.Properties.Title = zoneBox.Properties.Name;
                zoneDiagram.Properties.Description = zoneBox.Properties.Description;

                int num = 1;
                foreach (var port in zoneBox.Ports)
                {
                    if (port.IsConnected && !string.IsNullOrEmpty(port.ConnectShapeId))
                    {
                        var tline = areaDiagram.FindShape(port.ConnectShapeId) as TransmisionLine;
                        var name = tline?.Properties.Name;
                        var lineFeeder = new LineFeeder(name, num * 100, 80);
                        lineFeeder.AreaId = areaDiagram.Id;
                        lineFeeder.ZoneId = zoneDiagram.Id;
                        lineFeeder.Properties.LineFeederType = tline.Properties.LineFeederType;
                        lineFeeder.Properties.DispachingCode = tline.Properties.DispachingCode;
                        lineFeeder.Properties.IgmcCode = tline.Properties.IgmcCode;
                        lineFeeder.Properties.ToolId = tline.Properties.ToolId;
                        lineFeeder.Properties.TransferCapacity = tline.Properties.TransferCapacity;
                        lineFeeder.ConnectPortId = port.Id;
                        lineFeeder.ConnectShapeId = tline.Id;

                        zoneDiagram.AddShape(lineFeeder);
                        num++;
                    }
                }

                zone.Update(zoneDiagram);
                //zone.Diagram = JsonConvert.SerializeObject(zoneDiagram);
                await _context.Zones.AddAsync(zone, cancellationToken);
            }
            else
            {
                //zone.Name = zoneBox.Properties.Name;

                DiagramModel zoneDiagram = zone.Diagram;

                zoneDiagram.Properties.Title = zoneBox.Properties.Name;
                zoneDiagram.Properties.Description = zoneBox.Properties.Description;

                var lineFeeders = zoneDiagram.GetLineFeeders();
                List<LineFeeder> newLineFeeders = new List<LineFeeder>();

                foreach (var lf in lineFeeders)
                {
                    if (zoneBox.Ports.Any(port => port.Id == lf.ConnectPortId))
                        newLineFeeders.Add(lf);
                    else
                    {
                        Tuple<string, string> tuple = zoneDiagram.DeleteLineFeeder(lf);

                        if (!string.IsNullOrEmpty(tuple.Item1) && !string.IsNullOrEmpty(tuple.Item2))
                            await DeleteSubstationLineFeeders(tuple.Item1, tuple.Item2, cancellationToken);
                    }
                }


                int num = lineFeeders.Count() + 1;

                foreach (var port in zoneBox.Ports)
                {
                    if (port.IsConnected && !string.IsNullOrEmpty(port.ConnectShapeId))
                    {
                        LineFeeder lineFeeder = null;

                        if (newLineFeeders.Any(x => x.ConnectPortId == port.Id))
                        {
                            lineFeeder = newLineFeeders.SingleOrDefault(x => x.ConnectPortId == port.Id);
                            var tline = areaDiagram.FindShape(port.ConnectShapeId) as TransmisionLine;
                            var name = tline?.Properties.Name;
                            lineFeeder.Properties.Name = name;
                            lineFeeder.Properties.LineFeederType = tline.Properties.LineFeederType;
                            lineFeeder.Properties.DispachingCode = tline.Properties.DispachingCode;
                            lineFeeder.Properties.IgmcCode = tline.Properties.IgmcCode;
                            lineFeeder.Properties.ToolId = tline.Properties.ToolId;
                            lineFeeder.Properties.TransferCapacity = tline.Properties.TransferCapacity;
                            lineFeeder.SetText("Name", name);

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
                                await UpdateSubstationLineFeeders(tuple.Item1, tuple.Item2, connLine, cancellationToken);
                            }
                        }
                        else
                        {
                            var tline = areaDiagram.FindShape(port.ConnectShapeId) as TransmisionLine;
                            var name = tline?.Properties.Name;
                            lineFeeder = new LineFeeder(name, num * 100, 80);
                            lineFeeder.AreaId = areaDiagram.Id;
                            lineFeeder.ZoneId = zoneDiagram.Id;
                            lineFeeder.Properties.LineFeederType = tline.Properties.LineFeederType;
                            lineFeeder.Properties.DispachingCode = tline.Properties.DispachingCode;
                            lineFeeder.Properties.IgmcCode = tline.Properties.IgmcCode;
                            lineFeeder.Properties.ToolId = tline.Properties.ToolId;
                            lineFeeder.Properties.TransferCapacity = tline.Properties.TransferCapacity;
                            lineFeeder.ConnectPortId = port.Id;
                            lineFeeder.ConnectShapeId = tline.Id;

                            zoneDiagram.AddShape(lineFeeder);
                            num++;
                        }
                    }
                }

                //zone.Diagram = JsonConvert.SerializeObject(zoneDiagram);
                zone.Update(zoneDiagram);
                _context.Zones.Update(zone);
            }
        }
    }

    private void DeleteZone(Zone zone)
    {
        DiagramModel zoneDiagram = zone.Diagram;
        var lineFeeders = zoneDiagram.GetLineFeeders();

        foreach (var lf in lineFeeders)
        {
            zoneDiagram.RemoveShape(lf);
        }
        _context.Zones.Remove(zone);
    }

    private async Task UpdateSubstationLineFeeders(string substationId, string portId, TransmisionLine tline, CancellationToken cancellationToken)
    {
        if (substationId == string.Empty || portId == string.Empty)
        {
            throw new ArgumentNullException("Error ZoneId or portId is Empty");
        }

        var substationIdGuid = new Guid(substationId);
        Substation substation = await _context.Substations.SingleOrDefaultAsync(z => z.Id == substationIdGuid, cancellationToken);

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

    private async Task DeleteSubstationLineFeeders(string substationId, string portId, CancellationToken cancellationToken)
    {
        var substationIdGuid = new Guid(substationId);
        Substation substation = await _context.Substations.SingleOrDefaultAsync(z => z.Id == substationIdGuid, cancellationToken);

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
