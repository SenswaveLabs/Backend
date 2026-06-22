using MassTransit.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Senswave.DataSources.Domain.Brokers.Brokers.Repositories;
using Senswave.DataSources.Domain.Brokers.Clients.Proxy;
using Senswave.DataSources.Domain.Brokers.Sessions.Repositories;
using Senswave.DataSources.Infrastructure.Brokers.Brokers.Features;
using Senswave.DataSources.Infrastructure.Brokers.Brokers.Repositories;
using Senswave.DataSources.Infrastructure.Brokers.Clients.Features.ClientLogging;
using Senswave.DataSources.Infrastructure.Brokers.Clients.Proxy;
using Senswave.DataSources.Infrastructure.Brokers.Features;
using Senswave.DataSources.Infrastructure.Brokers.Sessions.Repositories;
using Senswave.DataSources.Infrastructure.Consumers;
using Senswave.DataSources.Infrastructure.DataSources.Consumers;

namespace Senswave.DataSources.Infrastructure;

public static class DataSourcesExtensions
{
    public static IServiceCollection AddDataSourcesInfrastructure(this IServiceCollection services)
    {
        services.AddDbContext<DataSourcesContext>();

        services.AddScoped<IClientProxy, ClientProxy>();

        services.AddScoped<IBrokerCommandRepository, BrokerCommandRepository>();
        services.AddScoped<IBrokerQueryRepository, BrokerQueryRepository>();
        services.AddScoped<ISubscribtionCommandRepository, SubscribtionCommandRepository>();
        services.AddScoped<ISubscribtionQueryRepository, SubscribtionQueryRepository>();

        services.AddScoped<ISessionCommandRepository, SessionCommandRepository>();
        services.AddScoped<ISessionQueryRepository, SessionQueryRepository>();

        services.RegisterConsumer<DataSourcesRemoveConsumer>();

        services.RegisterConsumer<PublishMessageToDeviceConsumer>();
        services.RegisterConsumer<CreateSubscriptionConsumer>();
        services.RegisterConsumer<DeleteAllSubscribtionsConsumer>();
        services.RegisterConsumer<BrokerAccessConsumer>();
        services.RegisterConsumer<GetSubscribtionTopicConsumer>();
        services.RegisterConsumer<MessageReceivedConsumer>();

        services.RegisterConsumer<ClientConnectedLogHandler>();
        services.RegisterConsumer<ClientDisconnectedLogHandler>();
        services.RegisterConsumer<ClientReconnectingLogHandler>();
        services.RegisterConsumer<MessageIgnoredLogHandler>();

        services.RegisterConsumer<DataSourcesStateConsumer>();

        return services;
    }
}
