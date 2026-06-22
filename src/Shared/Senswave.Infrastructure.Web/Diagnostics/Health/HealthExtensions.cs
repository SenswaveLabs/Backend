using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Senswave.Infrastructure.Web.Diagnostics.Health;

public static class HealthExtensions
{
    public static IServiceCollection AddHealth(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<HealthCheckMiddleware>();
        services.Configure<HealthOptions>(configuration.GetSection(HealthOptions.SectionName));

        return services;
    }
}
