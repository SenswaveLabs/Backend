using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Senswave.DataSources.Domain.Brokers.Brokers.Options;
using Senswave.DataSources.Domain.Diagnostics;

namespace Senswave.DataSources.Domain;

public static class DataSourcesExtensions
{
    public static IServiceCollection AddDataSourcesDomain(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<BrokerOptions>(configuration.GetSection(BrokerOptions.SectionName));

        services.AddSingleton<IDataSourcesActivityProvider, DataSourcesActivityProvider>();

        return services;
    }
}
