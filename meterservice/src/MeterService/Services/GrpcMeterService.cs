using MeterService.Application.MeterEnergy.Commands;
using MeterService.Application.MeterEnergy.Queries;
using MeterService.Application.Meters.Commands;
using MeterService.Application.Meters.Queries;
using MeterService.gRPC;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using MediatR;

namespace MeterService.Services;

public sealed class GrpcMeterService : gRPC.MeterService.MeterServiceBase
{
    private readonly ISender _mediator;

    public GrpcMeterService(ISender mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public override async Task<Meter> GetMeter(MeterRequest request, ServerCallContext context)
    {
        var result = await _mediator.Send(new GetMeterQuery() { SerialNumber = request.SerialNumber}, context.CancellationToken);
        var meter = new Meter()
        {
            MeterId = result.MeterId,
            SerialNumber = result.SerialNumber,
            Name = result.Name,
            StationId = result.StationId ?? 0,
            StationName = result.StationName,
            Active = result.Active,
            Dismount = result.Dismount,
            StartOperationDate = result.StartOperationDate.ToTimestamp(),
            EndOperationDate = result.EndOperationDate.ToTimestamp(),
        };

        return meter;
    }

    public override async Task<MetersResponse> GetMeters(Empty request, ServerCallContext context)
    {
        var metersResponse = new MetersResponse();

        var result = await _mediator.Send(new GetMetersQuery() , context.CancellationToken);

        foreach(var m in result)
        {
            var meter = new Meter()
            {
                MeterId = m.MeterId,
                SerialNumber = m.SerialNumber,
                Name = m.Name,
                StationId = m.StationId ?? 0,
                StationName = m.StationName,
                Active = m.Active,
                Dismount = m.Dismount,
                StartOperationDate = m.StartOperationDate.ToTimestamp(),
                EndOperationDate = m.EndOperationDate.ToTimestamp(),
            };

            metersResponse.Meters.Add(meter);
        }

        return metersResponse;
    }

    public override async Task<Empty> DeleteMeter(DeleteMeterRequest request, ServerCallContext context)
    {
        await _mediator.Send(new DeleteMeterCommand { MeterId = request.MeterId }, context.CancellationToken);
        return new Empty();
    }

    public override async Task<Empty> UpdateMeter(UpdateMeterRequest request, ServerCallContext context)
    {
        UpdateMeterCommand command = new UpdateMeterCommand
        {
            MeterId = request.Meter.MeterId,
            SerialNumber = request.Meter.SerialNumber,
            Name = request.Meter.Name,
            StationId = request.Meter.StationId,
            StationName = request.Meter.StationName,
            Active = request.Meter.Active,
            Dismount = request.Meter.Dismount,
            StartOperationDate = request.Meter.StartOperationDate.ToDateTimeOffset(),
            EndOperationDate = request.Meter.EndOperationDate.ToDateTimeOffset(),
        };
        await _mediator.Send(command, context.CancellationToken);
        return new Empty();
    }

    public override async Task<SubstationResponse> GetSubstations(Empty request, ServerCallContext context)
    {
        var substationsResponse = new SubstationResponse();

        var result = await _mediator.Send(new GetSubstationsQuery(), context.CancellationToken);

        foreach (var s in result)
        {
            if (s.Value.HasValue)
            {
                var substation = new Substation()
                {
                    Value = s.Value.Value,
                    Label = s.Label,
                };

                substationsResponse.Substations.Add(substation);
            }
        }

        return substationsResponse;
    }

    public override async Task<MetersResponse> GetSubstationMeters(SubstationMetersRequest request, ServerCallContext context)
    {
        var metersResponse = new MetersResponse();

        var result = await _mediator.Send(new GetSubstationMetersQuery { SubstationId = request.SubstationId }, context.CancellationToken);

        foreach (var m in result)
        {
            var meter = new Meter()
            {
                MeterId = m.MeterId,
                SerialNumber = m.SerialNumber,
                Name = m.Name,
                StationId = m.StationId ?? 0,
                StationName = m.StationName,
                Active = m.Active,
                Dismount = m.Dismount,
                StartOperationDate = m.StartOperationDate.ToTimestamp(),
                EndOperationDate = m.EndOperationDate.ToTimestamp(),
            };

            metersResponse.Meters.Add(meter);
        }

        return metersResponse;
    }

    public override async Task<BoolValue> UpdateMeterTable(Empty request, ServerCallContext context)
    {
        var result = await _mediator.Send(new UpdateMeterTableCommand(), context.CancellationToken);
        return new BoolValue() { Value = result };
    }

    public override async Task<MeterEnergyResponse> GetMetersEnergy(MetersEnergyRequest request, ServerCallContext context)
    {
        var meterEnergyResponse = new MeterEnergyResponse();

        var result = await _mediator.Send(new GetMetersEnergyQuery() {
            RecordDate = request.Date.ToDateTime() },
            context.CancellationToken);

        foreach (var me in result)
        {
            var meterEnergy = new MeterEnergy()
            {
                MeterEnergyId = me.MeterEnergyId.ToString(),
                MeterId = me.MeterId,
                RecordDate = me.RecordDate.ToTimestamp(),
                ToolTypeId = me.ToolTypeId ?? 0,
                EnergyActiveExport = me.EnergyActiveExport,
                EnergyActiveImport = me.EnergyActiveImport,
                EnergyReactiveExport = me.EnergyReactiveExport,
                EnergyReactiveImport = me.EnergyReactiveImport,
            };

            meterEnergyResponse.MeterEnergies.Add(meterEnergy);
        }

        return meterEnergyResponse;
    }

    public override async Task<MeterEnergyResponse> GetMeterEnergy(MeterEnergyRequest request, ServerCallContext context)
    {
        var meterEnergyResponse = new MeterEnergyResponse();

        var result = await _mediator.Send(
            new GetMeterEnergyQuery() {RecordDate = request.Date.ToDateTime(), SerialNumber = request.SerialNumber },
            context.CancellationToken);

        foreach (var me in result)
        {
            var meterEnergy = new MeterEnergy()
            {
                MeterEnergyId = me.MeterEnergyId.ToString(),
                MeterId = me.MeterId,
                RecordDate = me.RecordDate.ToTimestamp(),
                ToolTypeId = me.ToolTypeId ?? 0,
                EnergyActiveExport = me.EnergyActiveExport,
                EnergyActiveImport = me.EnergyActiveImport,
                EnergyReactiveExport = me.EnergyReactiveExport,
                EnergyReactiveImport = me.EnergyReactiveImport,
            };

            meterEnergyResponse.MeterEnergies.Add(meterEnergy);
        }

        return meterEnergyResponse;
    }

    public override async Task<Empty> DownloadMeterEnergy(DownloadMeterEnergyRequest request, ServerCallContext context)
    {
        var command = new DownloadMeterEnergyCommand()
        {
            StartDate = request.StartDate.ToDateTimeOffset(),
            EndDate = request.EndDate.ToDateTimeOffset(),
            SerialNumber = request.SerialNumber,
            Update = request.Update,
        };
        await _mediator.Send(command, context.CancellationToken);
        return new Empty();
    }

    public override async Task<Empty> DownloadMetersEnergy(DownloadMetersEnergyRequest request, ServerCallContext context)
    {
        var command = new DownloadMetersEnergyCommand()
        {
            StartDate = request.StartDate.ToDateTimeOffset(),
            EndDate = request.EndDate.ToDateTimeOffset(),
            Update = request.Update,
        };
        await _mediator.Send(command, context.CancellationToken);
        return new Empty();
    }
}
