using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MQTTnet;
using Senswave.DataSources.Application.Brokers.Brokers.Services;
using Senswave.DataSources.Domain.Brokers.Brokers.Services;

namespace Senswave.DataSources.Application;

public static class DataSourcesExtensions
{
    public static IServiceCollection AddDataSourcesApplication(this IServiceCollection services, IConfiguration configuration)
    {
        var assembly = typeof(DataSourcesExtensions).Assembly;
        services.AddMediatR(config => config.RegisterServicesFromAssembly(assembly));
        services.AddValidatorsFromAssembly(assembly);

        services.TryAddTransient<MqttFactory>();
        services.AddSingleton<IBrokerService, BrokerService>();

        return services;
    }
}
