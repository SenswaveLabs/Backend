using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Senswave.Homes.Domain;

public static class HomesExtensions
{
    public static IServiceCollection AddHomesDomain(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<HomeModuleOptions>(configuration.GetSection(HomeModuleOptions.SectionName));

        return services;
    }
}
