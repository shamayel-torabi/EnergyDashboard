using Application.Common.Models;
using MediatR;
using MeterFailService.Application.MeterEnergy.Commands;
using MeterFailService.Application.MeterEnergy.Queries;
using Microsoft.AspNetCore.Mvc;

namespace MeterFailService.Endpoints;

public static class MetersEnergyEndpoints
{
    public static void MapMetersEnergyEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/MetersEnergy").WithTags("MetersEnergy");

        group.MapGet("/{date}", async (
            [FromRoute] DateTime date,
            ISender mediator,
            CancellationToken token) =>
        {
            var query = new GetMetersEnergyQuery()
            {
                RecordDate = date
            };
            var result = await mediator.Send(query, token);

            if (result.IsSuccess)
            {
                return Results.Ok(result);
            }
            else
            {
                return Results.BadRequest(result);
            }
        })
        .Produces<IEnumerable<MeterEnergyDto>>()
        .WithName("GetMetersEnergy")
        .WithOpenApi();

        group.MapGet("/GetPaginatedMeterEnergy/{date}", async (
            [FromRoute] DateTimeOffset date,
            [FromQuery] int pageIndex,
            [FromQuery] int pageSize,
            [FromQuery] bool abnormal,
            ISender mediator,
            CancellationToken token) =>
        {
            var query = new GetPaginatedMeterEnergyQuery()
            {
                RecordDate = date,
                PageIndex = pageIndex,
                PageSize = pageSize,
                Abnormal = abnormal
            };
            var result = await mediator.Send(query, token);

            if (result.IsSuccess)
            {
                return Results.Ok(result);
            }
            else
            {
                return Results.BadRequest(result);
            }
        })
        .Produces<PaginatedList<MeterEnergyDto>>()
        .WithName("GetPaginatedMeterEnergy")
        .WithOpenApi();

        group.MapPost("/", async (
            [FromBody] DownloadMetersEnergyCommand command,
            ISender mediator,
            CancellationToken token) =>
        {
            var result = await mediator.Send(command, token);

            if (result.IsSuccess)
            {
                return Results.Ok(result);
            }
            else
            {
                return Results.BadRequest(result);
            }
        })
        .WithName("DownloadMetersEnergy")
        .WithOpenApi();


        group.MapPut("/{id}", async (
            [FromRoute] Guid id,
            [FromBody] MeterEnergyUpdateDto request,
            ISender mediator,
            CancellationToken token) =>
        {
            var command = new UpdateMetersEnergyCommand()
            {
                Id = id,
                Anomaly = request.Anomaly,
            };

            var result = await mediator.Send(command, token);

            if (result.IsSuccess)
            {
                return Results.NoContent();
            }
            else
            {
                return Results.BadRequest(result);
            }

        })
        .WithName("UpdateMeterEnergy")
        .WithOpenApi();
    }
}
