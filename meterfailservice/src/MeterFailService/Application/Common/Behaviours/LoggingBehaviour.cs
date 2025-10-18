using MediatR.Pipeline;

namespace MeterFailService.Application.Common.Behaviours;

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

        _logger.LogTrace("MeterFailService Request: {Name} {@Request}", requestName, request);
        return Task.CompletedTask;
    }
}
