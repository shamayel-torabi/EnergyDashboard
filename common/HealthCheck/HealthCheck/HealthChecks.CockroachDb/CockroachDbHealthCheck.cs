using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace HealthChecks.CockroachDb
{
    public class CockroachDbHealthCheck : IHealthCheck
    {
        private readonly string _serverUrl;
        private readonly TimeSpan _timeout;

        public CockroachDbHealthCheck(string serverUrl, TimeSpan? timeout)
        {
            _serverUrl = serverUrl ?? throw new ArgumentNullException(nameof(serverUrl));
            _timeout = timeout.HasValue ? timeout.Value : TimeSpan.FromSeconds(5);
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.Timeout = _timeout;
                    var checkingResponse = await client.GetAsync($"{_serverUrl}/health");

                    if (!checkingResponse.IsSuccessStatusCode)
                        return HealthCheckResult.Unhealthy("ارتباط با بانک اطلاعاتی با مشکل مواجه شده است!");
                    else
                        return HealthCheckResult.Healthy("ارتباط با بانک اطلاعاتی برقرار است");
                }
            }
            catch (Exception)
            {
                return HealthCheckResult.Unhealthy("ارتباط با بانک اطلاعاتی با مشکل مواجه شده است!");
            }
        }
    }
}
