using EnergyDashboard.Domain.Diagram;
using EnergyDashboard.Domain.Entities;
using EnergyDashboard.Domain.Repository;
using Microsoft.EntityFrameworkCore;
using Application.Common.Exceptions;
using EnergyDashboard.Domain.Exceptions;
using EnergyDashboard.Infrastructure.Persistence;

namespace EnergyDashboard.Infrastructure.Repository;

public class SubstationRepository : Repository<Guid, Substation> ,ISubstationRepository
{

    public SubstationRepository(AppDbContext context):base(context)
    {
    }

    public async Task<Substation> GetSubstationWithEquipments(Guid substationId, CancellationToken cancellationToken = default)
    {
        var substation = await _context.Substations
            .AsSplitQuery()
            .Include(i => i.Equipments)
            .SingleOrDefaultAsync(w => w.Id == substationId, cancellationToken);

        return substation;
    }

    public async Task UpdateDiagram(Guid id, DiagramModel substationDiagram, CancellationToken cancellationToken = default)
    {
        Substation substation = await _context.Substations
            .AsSplitQuery()
            .Include(i => i.Equipments)
            .SingleOrDefaultAsync(s => s.Id == id, cancellationToken);

        if (substation is null)
            throw new NotFoundException(nameof(Substation), id);

        substation.Update(substationDiagram);

        await UpdateEquipmens(substationDiagram, cancellationToken);

        Update(substation);
    }

    private async Task UpdateEquipmens(DiagramModel diagram, CancellationToken cancellationToken)
    {
        var equipments = diagram.GetEquipments();
        Guid diagramId = new Guid(diagram.Id);

        var eqps = await _context.Equipments.Where(w => w.SubstationId == diagramId).ToListAsync(cancellationToken);

        if (equipments is null)
        {
            _context.Equipments.RemoveRange(eqps);
            return;
        }

        foreach (var eq in eqps)
        {
            if (!equipments.Any(x => x.Id == eq.Id))
            {
                _context.Equipments.Remove(eq);
            }
        }

        foreach (var e in equipments)
        {
            if (_context.Equipments.Any(x => x.Id == e.Id))
            {
                switch (e.Type)
                {
                    case "Generator":
                        var ppf = await _context.GeneratorFeeders
                            .SingleOrDefaultAsync(x => x.Id == e.Id, cancellationToken);

                        if (ppf is not null)
                        {
                            ppf.Update(e as GeneratorFeederEquipment);
                            _context.GeneratorFeeders.Update(ppf);
                        }
                        break;
                    case "Transformer":
                        var tff = await _context.TransformerFeeders
                            .SingleOrDefaultAsync(x => x.Id == e.Id, cancellationToken);

                        if (tff is not null)
                        {
                            tff.Update(e as TransformerFeederEquipment);
                            _context.TransformerFeeders.Update(tff);
                        }
                        break;
                    case "LoadFeeder":
                        var lof = await _context.LoadFeeders
                            .SingleOrDefaultAsync(x => x.Id == e.Id, cancellationToken);

                        if (lof is not null)
                        {
                            lof.Update(e as LoadFeederEquipment);
                            _context.LoadFeeders.Update(lof);
                        }
                        break;
                    case "LineFeeder":
                        var lif = await _context.LineFeeders
                            .SingleOrDefaultAsync(x => x.Id == e.Id, cancellationToken);

                        if (lif is not null)
                        {
                            lif.Update(e as LineFeederEquipment);
                            _context.LineFeeders.Update(lif);
                        }
                        break;
                    default:
                        throw new DomainException(DomainErrors.DiagramModel.BadEquipmentType);

                }
            }
            else
            {
                switch (e.Type)
                {
                    case "Generator":
                        await _context.GeneratorFeeders.AddAsync(e as GeneratorFeederEquipment, cancellationToken);
                        break;
                    case "Transformer":
                        await _context.TransformerFeeders.AddAsync(e as TransformerFeederEquipment, cancellationToken);
                        break;
                    case "LoadFeeder":
                        await _context.LoadFeeders.AddAsync(e as LoadFeederEquipment, cancellationToken);
                        break;
                    case "LineFeeder":
                        await _context.LineFeeders.AddAsync(e as LineFeederEquipment, cancellationToken);
                        break;
                    default:
                        throw new DomainException(DomainErrors.DiagramModel.BadEquipmentType);
                }
            }
        }
    }
}
