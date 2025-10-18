using Grpc.Core;
using MeterService.gRPC.Services;

namespace Microsoft.Extensions.DependencyInjection;

public static class MessagingBuilderExtensions
{
    public static IServiceCollection AddGrpcMeterService(this IServiceCollection services, string address)
    {
        services.AddGrpcClient<MeterService.gRPC.MeterService.MeterServiceClient>(o =>
        {
            o.Address = new Uri(address);
        }).ConfigureChannel(o =>
        {
            o.Credentials = ChannelCredentials.Insecure;
        });

        services.AddTransient<IMeterClient, MeterCleint>();

        return services;
    }
}