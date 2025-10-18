using Microsoft.Extensions.Diagnostics.HealthChecks;
using StackExchange.Redis;
using System.Collections.Concurrent;

#nullable disable
namespace HealthChecks;

public class RedisHealthCheck : IHealthCheck
{
    private static readonly ConcurrentDictionary<string, ConnectionMultiplexer> _connections = new ConcurrentDictionary<string, ConnectionMultiplexer>();
    private readonly string _redisConnectionString;

    public RedisHealthCheck(string redisConnectionString)
    {
        _redisConnectionString = redisConnectionString ?? throw new ArgumentNullException(nameof(redisConnectionString));
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            if (!_connections.TryGetValue(_redisConnectionString, out ConnectionMultiplexer connection))
            {
                connection = await ConnectionMultiplexer.ConnectAsync(_redisConnectionString);

                if (!_connections.TryAdd(_redisConnectionString, connection))
                {
                    // Dispose new connection which we just created, because we don't need it.
                    connection.Dispose();
                    connection = _connections[_redisConnectionString];
                }
            }

            foreach (var endPoint in connection.GetEndPoints(configuredOnly: true))
            {
                var server = connection.GetServer(endPoint);

                if (server.ServerType != ServerType.Cluster)
                {
                    await connection.GetDatabase().PingAsync();
                    await server.PingAsync();
                }
                else
                {
                    var clusterInfo = await server.ExecuteAsync("CLUSTER", "INFO");

                    if (clusterInfo is object && !clusterInfo.IsNull)
                    {
                        if (!clusterInfo.ToString()
                            .Contains("cluster_state:ok"))
                        {
                            //cluster info is not ok!
                            return new HealthCheckResult(context.Registration.FailureStatus, description: $"کلاستر {endPoint} در وضعیت سالم نیست!");
                        }
                    }
                    else
                    {
                        //cluster info cannot be read for this cluster node 
                        return new HealthCheckResult(context.Registration.FailureStatus, description: $"کلاستر {endPoint} خالی یا قابل خواندن نیست!");
                    }
                }
            }

            return HealthCheckResult.Healthy($"کلاستر در وضعیت خوب قرار دارد.");
        }
        catch (Exception ex)
        {
            return new HealthCheckResult(context.Registration.FailureStatus, description: $"کلاستر در وضعیت سالم نیست!", exception: ex);
        }
    }
}
