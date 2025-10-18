using HealthChecks.CockroachDb;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Extension methods to configure <see cref="CockroachDbHealthCheck"/>.
    /// </summary>
    public static class CockroachDbHealthCheckBuilderExtensions
    {
        const string NAME = "CockroachDb";

        /// <summary>
        /// Add a health check for Postgres databases.
        /// </summary>
        /// <param name="builder">The <see cref="IHealthChecksBuilder"/>.</param>
        /// <param name="serverUrl">The CockroachDb Server Url to be used.</param>
        /// <param name="name">The health check name. Optional. If <c>null</c> the type name 'npgsql' will be used for the name.</param>
        /// <param name="failureStatus">
        /// The <see cref="HealthStatus"/> that should be reported when the health check fails. Optional. If <c>null</c> then
        /// the default status of <see cref="HealthStatus.Unhealthy"/> will be reported.
        /// </param>
        /// <param name="tags">A list of tags that can be used to filter sets of health checks. Optional.</param>
        /// <param name="timeout">An optional <see cref="TimeSpan"/> representing the timeout of the check.</param>
        /// <returns>The specified <paramref name="builder"/>.</returns>
        public static IHealthChecksBuilder AddCockroachDb(
            this IHealthChecksBuilder builder,
            string serverUrl,
            string? name = default,
            HealthStatus? failureStatus = default,
            IEnumerable<string>? tags = default,
            TimeSpan? timeout = default)
        {
            if (serverUrl == null)
            {
                throw new ArgumentNullException(nameof(serverUrl));
            }
            builder.Services.AddSingleton(sp => new CockroachDbHealthCheck(serverUrl, timeout));

            return builder.Add(new HealthCheckRegistration(
                name ?? NAME,
                sp => sp.GetRequiredService<CockroachDbHealthCheck>(),
                failureStatus,
                tags,
                timeout));
        }
    }
}
