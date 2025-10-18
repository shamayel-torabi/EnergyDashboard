using Application.Common.Exceptions;
using Domain.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace EnergyDashboard.Middleware;


public static class ExceptionHandler
{
    public static void Handle(IApplicationBuilder app)
    {
        app.Run(async context =>
        {
            var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
            Exception exception = exceptionHandlerPathFeature.Error;

            ProblemDetails problem = exception switch
            {
                ValidationException validationException => new ValidationProblemDetails(validationException.Errors)
                {
                    Title = validationException.Message,
                    Status = (int)HttpStatusCode.BadRequest
                },
                NotFoundException notFoundException => new ProblemDetails
                {
                    Title = notFoundException.Message,
                    Status = (int)HttpStatusCode.NotFound
                },
                DomainException domainException => new ProblemDetails
                {
                    Title = domainException.Message,
                    Status = (int)HttpStatusCode.BadRequest
                },
                _ => new ProblemDetails
                {
                    Title = exception.Message,
                    Status = (int)HttpStatusCode.InternalServerError
                }
            };

            context.Response.StatusCode = problem.Status.Value;
            await Results.Problem(problem).ExecuteAsync(context);
        });
    }
}
