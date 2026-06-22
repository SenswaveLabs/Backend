using MassTransit.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MQTTnet;
using Senswave.DataSources.BrokerConnection.Factories;
using Senswave.DataSources.BrokerConnection.Features.Cleanup;
using Senswave.DataSources.BrokerConnection.Features.ClientExists;
using Senswave.DataSources.BrokerConnection.Features.GetClientState;
using Senswave.DataSources.BrokerConnection.Features.PublishMessage;
using Senswave.DataSources.BrokerConnection.Features.Restart;
using Senswave.DataSources.BrokerConnection.Features.Start;
using Senswave.DataSources.BrokerConnection.Features.Status;
using Senswave.DataSources.BrokerConnection.Features.Stop;
using Senswave.DataSources.BrokerConnection.Features.Subscribe;
using Senswave.DataSources.BrokerConnection.Features.Unsubscribe;
using Senswave.DataSources.BrokerConnection.Interfaces;
using Senswave.DataSources.BrokerConnection.RateLimiters;
using Senswave.DataSources.BrokerConnection.Services;
using Senswave.DataSources.Domain.Brokers.Clients.Options;

namespace Senswave.DataSources.BrokerConnection;

public static class DataSourcesExtensions
{
    public static IServiceCollection AddBrokerClientServices(this IServiceCollection services, IConfiguration configuration)
    {
        var assembly = typeof(DataSourcesExtensions).Assembly;
        services.AddMediatR(config => config.RegisterServicesFromAssembly(assembly));

        services.RegisterConsumer<RestartClientConsumer>();
        services.RegisterConsumer<PublishMessageConsumer>();
        services.RegisterConsumer<StartClientConsumer>();
        services.RegisterConsumer<StopClientConsumer>();
        services.RegisterConsumer<SubscribeConsumer>();
        services.RegisterConsumer<UnsubscribeConsumer>();
        services.RegisterConsumer<ClientExistsConsumer>();
        services.RegisterConsumer<ClientStateConsumer>();
        services.RegisterConsumer<WorkerStatusConsumer>();

        services.TryAddTransient<MqttFactory>();
        services.AddTransient<ClientFactory>();

        services.AddSingleton<IClientService, ClientService>();
        services.AddTransient<IGracefulStopClientService, GracefulStopClientService>();
        services.AddSingleton<ICleanupService, CleanupService>();
        services.AddHostedService<CleanupHostedService>();

        services.Configure<ClientCleanupOptions>(configuration.GetSection(ClientCleanupOptions.SectionName));

        services.Configure<RateLimitersOptions>(configuration.GetSection(RateLimitersOptions.SectionName));

        return services;
    }
}
