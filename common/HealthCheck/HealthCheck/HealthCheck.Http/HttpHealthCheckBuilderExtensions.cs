using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

#nullable disable

namespace HealthCheck;

public static class NetworkHealthCheckBuilderExtensions
{
    const string HTTP_NAME = "بررسی اتصال شبکه";
    public static IHealthChecksBuilder AddHttpHealthCheck(this IHealthChecksBuilder builder, Action<HttpHealthCheckOptions> setup, string name = default, HealthStatus? failureStatus = default, IEnumerable<string> tags = default, TimeSpan? timeout = default)
    {
        var options = new HttpHealthCheckOptions();
        setup?.Invoke(options);

        return builder.Add(new HealthCheckRegistration(
           name ?? HTTP_NAME,
           sp => new HttpHealthCheck(options),
           failureStatus,
           tags,
           timeout));
    }

}
