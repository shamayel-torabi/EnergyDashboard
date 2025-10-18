using Application.Common.Exceptions;
using EnergyDashboard.Domain.Entities;
using EnergyDashboard.Domain.Diagram;
using EnergyDashboard.Domain.Repository;
using EnergyDashboard.Application.Interfaces;
using EnergyDashboard.Domain.Diagram.Shapes;
using MediatR;

namespace EnergyDashboard.Application.Substations.Queries;

public class GetSubstationDiagramQuery : IRequest<DiagramModel>
{
    public Guid Id { get; set; }
}

public class GetSubstationDiagramQueryHandler : IRequestHandler<GetSubstationDiagramQuery, DiagramModel>
{
    private readonly ISubstationRepository _substationRepository;
    private readonly IMeterInstantService _meterInstantService;

    public GetSubstationDiagramQueryHandler(
        ISubstationRepository substationRepository,
        IMeterInstantService meterInstantService)
    {
        _substationRepository = substationRepository;
        _meterInstantService = meterInstantService;
    }

    public async Task<DiagramModel> Handle(GetSubstationDiagramQuery request, CancellationToken cancellationToken)
    {
        var substation = await _substationRepository.GetByIdAsync( request.Id , cancellationToken);

        if (substation is null)
            throw new NotFoundException(nameof(Substation), request.Id);

        DiagramModel diagram = substation.Diagram;
        await UpdateMeterInstant(diagram, cancellationToken);

        return diagram;
    }

    private async Task UpdateMeterInstant(DiagramModel diagram, CancellationToken cancellationToken)
    {
        IList<BusbarModel> busbarModels = new List<BusbarModel>();
        busbarModels = diagram.GetBusbarModels();

        foreach (var b in busbarModels)
        {
            var bus = diagram.Shapes.FirstOrDefault(x => x.Id == b.BusbarId.ToString()) as BusBar;
            double busLoss = 0.0;

            foreach (var eq in b.Equipments)
            {
                var shape = diagram.Shapes.FirstOrDefault(x => x.Id == eq.Id.ToString());
                var meter = eq.GetActiveMeter();

                if (meter is not null && shape is not null)
                {
                    var mi = await _meterInstantService.GetMeterInstantAsync(meter.SerialNumber, cancellationToken);
                    if (mi is not null)
                    {
                        var PowerActiveExport = mi.PExport;
                        var PowerActiveImport = mi.PImport;
                        var PowerReactiveExport = mi.QExport;
                        var PowerReactiveImport = mi.QImport;
                        double P = 0.0, Q = 0.0;

                        if (eq.Reverse)
                        {
                            P = (double)(PowerActiveExport - PowerActiveImport);
                            Q = (double)(PowerReactiveExport - PowerReactiveImport);
                        }
                        else
                        {
                            P = (double)(PowerActiveImport - PowerActiveExport);
                            Q = (double)(PowerReactiveImport - PowerReactiveExport);
                        }

                        shape.P = P;
                        shape.Q = Q;
                        busLoss += P;

                        double v = (double)(mi.VoltageA + mi.VoltageB + mi.VoltageC) * Math.Sqrt(3) / 3000;

                        if (mi.SerialNumber.StartsWith("3700") || mi.SerialNumber.StartsWith("5007"))
                        {
                            v *= eq.PTRatio.Value;
                        }

                        double vDif = Math.Abs(v - bus.Properties.VoltageMag) / bus.Properties.VoltageMag;

                        if (vDif < 0.2)
                            bus.Voltage = v;
                    }
                    else
                    {
                        shape.P = 0.0;
                        shape.Q = 0.0;
                    }
                }
            }
            bus.Loss = busLoss;
        }
    }
}
