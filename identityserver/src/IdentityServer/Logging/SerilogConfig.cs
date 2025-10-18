using Microsoft.Extensions.Hosting;
using Serilog;

namespace EnergyDashboard.Common.Logging;

public class SerilogConfig
{
    public static void LogConfigure(HostBuilderContext ctx, LoggerConfiguration lc)
    {
        var seqServerUrl = string.IsNullOrWhiteSpace(ctx.Configuration["Serilog:SeqServerUrl"]) ? "http://seq-logger" : ctx.Configuration["Serilog:SeqServerUrl"];
        lc.ReadFrom.Configuration(ctx.Configuration)
            .Enrich.FromLogContext()
            .Enrich.WithMachineName()
            .WriteTo.Console()
#if RELEASE
            .WriteTo.Seq(seqServerUrl);
#else
            .WriteTo.File(@"Log/log-.txt", rollingInterval: RollingInterval.Day);
#endif
    }
}
