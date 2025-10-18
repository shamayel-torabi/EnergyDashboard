using MediatR.Pipeline;

namespace MeterService.Application.Common.Behaviours;

public sealed class LoggingBehaviour<TRequest> : IRequestPreProcessor<TRequest>
{
    private readonly ILogger _logger;

    public LoggingBehaviour(ILogger<TRequest> logger)
    {
        _logger = logger;
    }

    public Task Process(TRequest request, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;

        _logger.LogTrace("MeterService Request: {Name} {@Request}", requestName, request);
        return Task.CompletedTask;
    }
}
