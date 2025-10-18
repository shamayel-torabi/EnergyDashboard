using System;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace HealthCheck;

public class HttpHealthCheck : IHealthCheck
{
    private readonly HttpHealthCheckOptions _options;
    public HttpHealthCheck(HttpHealthCheckOptions options)
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));
    }
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        var hosts = _options.ConfiguredHosts;
        bool error = false;

        Dictionary<string, object> properties = new Dictionary<string, object>();

        foreach (var host in hosts)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.Timeout = TimeSpan.FromSeconds(10.0);
                    var checkingResponse = await client.GetAsync(host.Value);

                    if (!checkingResponse.IsSuccessStatusCode)
                    {
                        properties.Add( $"ارتباط با وبگاه {host.Key} با خطا مواجه شده است.", false);
                        error = true;
                    }
                    else
                        properties.Add( $"ارتباط با وبگاه {host.Key} با موفقیت برقرار شده است.", true);
                }
            }
            catch (Exception)
            {
                properties.Add( $"ارتباط با وبگاه {host.Key} با خطا مواجه شده است.", false);
                error = true;
            }
        }

        var status = error ?  context.Registration.FailureStatus : HealthStatus.Healthy;
        var description = error ? "ارتباط شبکه با مشکل مواجه شده است!!" : "ارتباط شبکه با موفقیت برقرار است";
        return new HealthCheckResult(status, description: description,  data: properties);
    }
}
