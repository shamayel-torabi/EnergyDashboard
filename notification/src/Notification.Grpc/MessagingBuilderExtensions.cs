using Grpc.Core;
using Microsoft.Extensions.Logging;
using Notification.gRPC.Services;

namespace Microsoft.Extensions.DependencyInjection;

public static class MessagingBuilderExtensions
{
    public static IServiceCollection AddMessageService(this IServiceCollection services, string address)
    {
        services.AddGrpcClient<Notification.gRPC.Notification.NotificationClient>(o =>
        {
            o.Address = new Uri(address);
        }).ConfigureChannel(o =>
        {
            o.Credentials = ChannelCredentials.Insecure;
        });

        services.AddSingleton<IMessageService, MessageService>(sp =>
        {
            var loggerFactory = sp.GetRequiredService<ILoggerFactory>();
            var logger = loggerFactory.CreateLogger<MessageService>();
            var client = sp.GetRequiredService<Notification.gRPC.Notification.NotificationClient>();
            return new MessageService(sp, logger);
        });

        return services;
    }
}