using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;

#nullable disable

namespace HealthCheck;

public class MemoryHealthCheck : IHealthCheck
{
    private readonly IOptionsMonitor<MemoryCheckOptions> _options;
    public MemoryHealthCheck(IOptionsMonitor<MemoryCheckOptions> options)
    {
        _options = options;
    }
    public string Name => "memory_check";

    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default(CancellationToken))
    {
        var options = _options.Get(context.Registration.Name);

        // Include GC information in the reported diagnostics.
        var allocated = GC.GetTotalMemory(forceFullCollection: false);
        var data = new Dictionary<string, object>()
        {
            { $"حافظه تخصیص یافته : {allocated / (1024L * 1024L)} مگا بایت", true }
        };
        var status = (allocated < options.Threshold) ?
            HealthStatus.Healthy : context.Registration.FailureStatus;

        var description = $"چنانچه حافظه تخصیص یافته بزرگتر از {options.Threshold / (1024L * 1024L)}  مگا بایت باشد با کاهش عملکرد سرور مواجه خواهید شد.";

        return Task.FromResult(new HealthCheckResult(
            status,
            description: description,
            exception: null,
            data: data));
    }
}

public static class GCInfoHealthCheckBuilderExtensions
{
    public static IHealthChecksBuilder AddMemoryHealthCheck(
        this IHealthChecksBuilder builder,
        string name,
        HealthStatus? failureStatus = null,
        long? thresholdInBytes = null,
        IEnumerable<string> tags = null)
    {
        // Register a check of type GCInfo.
        builder.AddCheck<MemoryHealthCheck>(
            name, failureStatus ?? HealthStatus.Degraded, tags);

        // Configure named options to pass the threshold into the check.
        if (thresholdInBytes.HasValue)
        {
            builder.Services.Configure<MemoryCheckOptions>(name, options =>
            {
                options.Threshold = thresholdInBytes.Value;
            });
        }

        return builder;
    }
}

public class MemoryCheckOptions
{
    // Failure threshold (in bytes)
    public long Threshold { get; set; } = 1024L * 1024L * 1024L;
}
