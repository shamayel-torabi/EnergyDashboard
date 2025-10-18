
namespace HealthCheck;

public class HttpHealthCheckOptions
{
    internal Dictionary<string, Uri> ConfiguredHosts = new Dictionary<string, Uri>();
    public HttpHealthCheckOptions AddHost(string hostName, Uri uri)
    {
        ConfiguredHosts.Add(hostName, uri);
        return this;
    }
}
