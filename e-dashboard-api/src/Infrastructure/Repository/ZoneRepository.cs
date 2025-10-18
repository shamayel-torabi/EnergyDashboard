using EnergyDashboard.Domain.Diagram;
using EnergyDashboard.Domain.Entities;
using EnergyDashboard.Domain.Repository;
using Microsoft.EntityFrameworkCore;
using Application.Common.Exceptions;
using EnergyDashboard.Infrastructure.Persistence;
using Newtonsoft.Json;

namespace EnergyDashboard.Infrastructure.Repository;

public class ZoneRepository :Repository<Guid, Zone>, IZoneRepository
{
    public ZoneRepository(AppDbContext context):base(context)
    {
    }

    public async Task UpdateDiagram(Guid id, DiagramModel zoneDiagram, CancellationToken cancellationToken = default)
    {
        Zone zone = await _context.Zones.FindAsync(new object[] { id }, cancellationToken);

        if (zone is null)
            throw new NotFoundException(nameof(Zone), id);

        zone.Update(zoneDiagram);
        await UpdateSubstations(zoneDiagram, cancellationToken);
        Update(zone);
    }

    private async Task UpdateSubstations(DiagramModel zoneDiagram, CancellationToken cancellationToken)
    {
        var substationBoxes = zoneDiagram.GetBoxedConnectShapes();
        var zoneSubstations = await _context.Substations.Where(s => s.ZoneId.ToString() == zoneDiagram.Id).ToListAsync(cancellationToken);

        foreach (var s in zoneSubstations)
        {
            if (!substationBoxes.Any(bx => bx.Id == s.Id.ToString()))
            {
                _context.Substations.Remove(s);
                //DeleteSubstation(s);
            }
        }

        foreach (var substationBoxe in substationBoxes)
        {
            var substationId = new Guid(substationBoxe.Id);
            Substation substation = await _context.Substations.FirstOrDefaultAsync(n => n.Id == substationId, cancellationToken);

            if (substation is null)
            {
                substation = new Substation(substationId, new Guid(zoneDiagram.Id));

                var substationDiagram = new DiagramModel(substationBoxe.Id, zoneDiagram.Id, "substation");
                substationDiagram.Properties.Title = substationBoxe.Properties.Name;
                substationDiagram.Properties.Description = substationBoxe.Properties.Description;
                substationDiagram.Properties.IgmcStationId = substationBoxe.Properties.IgmcStationId;

                int num = 1;
                foreach (var port in substationBoxe.Ports)
                {
                    if (port.IsConnected && !string.IsNullOrEmpty(port.ConnectShapeId))
                    {
                        var tline = zoneDiagram.FindShape(port.ConnectShapeId) as Domain.Diagram.Shapes.TransmisionLine;
                        var name = tline?.Properties.Name;
                        var lineFeeder = new Domain.Diagram.Shapes.LineFeeder(name, num * 100, 80);
                        lineFeeder.SubstationId = substationBoxe.Id;
                        lineFeeder.ZoneId = zoneDiagram.Id;
                        lineFeeder.AreaId = zoneDiagram.ParentId;
                        lineFeeder.Properties.LineFeederType = tline.Properties.LineFeederType;
                        lineFeeder.Properties.DispachingCode = tline.Properties.DispachingCode;
                        lineFeeder.Properties.IgmcCode = tline.Properties.IgmcCode;
                        lineFeeder.Properties.ToolId = tline.Properties.ToolId;
                        lineFeeder.Properties.TransferCapacity = tline.Properties.TransferCapacity;
                        lineFeeder.ConnectPortId = port.Id;
                        lineFeeder.ConnectShapeId = tline.Id;

                        substationDiagram.AddShape(lineFeeder);
                        num++;
                    }
                }

                substation.Name = substationBoxe.Properties.Name;

                substation.Update(substationDiagram);
                await _context.Substations.AddAsync(substation, cancellationToken);
            }
            else
            {
                substation.Name = substationBoxe.Properties.Name;

                DiagramModel substationDiagram = substation.Diagram;

                substationDiagram.Properties.Title = substationBoxe.Properties.Name;
                substationDiagram.Properties.Description = substationBoxe.Properties.Description;
                substationDiagram.Properties.IgmcStationId = substationBoxe.Properties.IgmcStationId;

                var lineFeeders = substationDiagram.GetLineFeeders();
                List<Domain.Diagram.Shapes.LineFeeder> newLineFeeders = new List<Domain.Diagram.Shapes.LineFeeder>();

                foreach (var lf in lineFeeders)
                {
                    if (substationBoxe.Ports.Any(port => port.Id == lf.ConnectPortId))
                        newLineFeeders.Add(lf);
                    else
                    {
                        substationDiagram.RemoveShape(lf);
                    }
                }


                int num = lineFeeders.Count() + 1;

                foreach (var port in substationBoxe.Ports)
                {
                    if (port.IsConnected && !string.IsNullOrEmpty(port.ConnectShapeId))
                    {
                        Domain.Diagram.Shapes.LineFeeder lineFeeder = null;

                        if (newLineFeeders.Any(x => x.ConnectPortId == port.Id))
                        {
                            lineFeeder = newLineFeeders.SingleOrDefault(x => x.ConnectPortId == port.Id);
                            var tline = zoneDiagram.FindShape(port.ConnectShapeId) as Domain.Diagram.Shapes.TransmisionLine;
                            var name = tline?.Properties.Name;
                            lineFeeder.Properties.Name = name;
                            lineFeeder.Properties.DispachingCode = tline.Properties.DispachingCode;
                            lineFeeder.Properties.IgmcCode = tline.Properties.IgmcCode;
                            lineFeeder.Properties.ToolId = tline.Properties.ToolId;
                            lineFeeder.Properties.TransferCapacity = tline.Properties.TransferCapacity;
                            lineFeeder.SetText("Name", name);

                            var connLine = substationDiagram.FindLineFeederConnectedShape(lineFeeder) as Domain.Diagram.Shapes.TransmisionLine;
                            if (connLine is not null)
                            {
                                connLine.Properties.Name = lineFeeder.Properties.Name;
                                connLine.Properties.LineFeederType = lineFeeder.Properties.LineFeederType;
                                connLine.Properties.DispachingCode = lineFeeder.Properties.DispachingCode;
                                connLine.Properties.IgmcCode = lineFeeder.Properties.IgmcCode;
                                connLine.Properties.ToolId = lineFeeder.Properties.ToolId;
                                connLine.Properties.TransferCapacity = lineFeeder.Properties.TransferCapacity;
                            }
                        }
                        else
                        {
                            var tline = zoneDiagram.FindShape(port.ConnectShapeId) as Domain.Diagram.Shapes.TransmisionLine;
                            var name = tline?.Properties.Name;
                            lineFeeder = new Domain.Diagram.Shapes.LineFeeder(name, num * 100, 80);
                            lineFeeder.SubstationId = substationBoxe.Id;
                            lineFeeder.ZoneId = zoneDiagram.Id;
                            lineFeeder.AreaId = zoneDiagram.ParentId;
                            lineFeeder.Properties.LineFeederType = tline.Properties.LineFeederType;
                            lineFeeder.Properties.DispachingCode = tline.Properties.DispachingCode;
                            lineFeeder.Properties.IgmcCode = tline.Properties.IgmcCode;
                            lineFeeder.Properties.ToolId = tline.Properties.ToolId;
                            lineFeeder.Properties.TransferCapacity = tline.Properties.TransferCapacity;
                            lineFeeder.ConnectPortId = port.Id;
                            lineFeeder.ConnectShapeId = tline.Id;

                            substationDiagram.AddShape(lineFeeder);
                            num++;
                        }
                    }
                }

                substation.Update(substationDiagram);
                _context.Substations.Update(substation);
            }
        }
    }
}
