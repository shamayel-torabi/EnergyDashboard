using EnergyDashboard.Application.PowerplantTypes;
using EnergyDashboard.Application.PowerplantTypes.Commands;
using EnergyDashboard.Application.PowerplantTypes.Queries;
using MediatR;

namespace EnergyDashboard.Endpoints;

public static class PowerplantTypeEndpoints
{
    public static void MapPowerplantTypeEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/PowerplantType").WithTags("PowerplantTypes");

        group.MapGet("/", async (ISender _mediator, CancellationToken token) =>
        {
            var query = new GetPowerplantTypesQuery();

            var powerplantTypes = await _mediator.Send(query, token);

            return Results.Ok(powerplantTypes);
        })
        .WithName("GetAllPowerplantType")
        .WithOpenApi();

        group.MapGet("/{id}", async (int id, ISender _mediator, CancellationToken token) =>
        {
            var query = new GetPowerplantTypeQuery()
            {
                Id = id
            };

            var powerplantType = await _mediator.Send(query, token);

            return Results.Ok(powerplantType);
        })
        .WithName("GetPowerplantTypeById")
        .WithOpenApi();

        group.MapPut("/{id}", async (int id, PowerplantTypeDto input, ISender _mediator, CancellationToken token) =>
        {
            if (id != input.Id)
            {
                return Results.BadRequest();
            }

            var command = new UpdatePowerplantTypeCommand()
            {
                Id = id,
                Name = input.Name,
            };

            await _mediator.Send(command, token);

            return TypedResults.NoContent();
        })
            .WithName("UpdatePowerplantType")
            .WithOpenApi()
            .RequireAuthorization(conf =>
            {
                conf.RequireRole("Admins");
            });

        group.MapPost("/", async (PowerplantTypeDto model, ISender _mediator, CancellationToken token) =>
        {
            var command = new CreatePowerplantTypeCommand()
            {
                Name = model.Name,
            };

            var powerplantType = await _mediator.Send(command, token);

            Results.Ok(powerplantType);
        })
            .WithName("CreatePowerplantType")
            .WithOpenApi()
            .RequireAuthorization(conf =>
            {
                conf.RequireRole("Admins");
            })
;

        group.MapDelete("/{id}", async (int id, ISender _mediator, CancellationToken token) =>
        {
            var command = new DeletePowerplantTypeCommand()
            {
                Id = id,
            };
            var powerplantType = await _mediator.Send(command, token);
            Results.Ok(powerplantType);
        })
            .WithName("DeletePowerplantType")
            .WithOpenApi()
            .RequireAuthorization(conf =>
            {
                conf.RequireRole("Admins");
            });
    }
}
