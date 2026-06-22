using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Senswave.Infrastructure.Diagnostics.OpenTelemetry;

public static class OpenTelemetryExtensions
{
    public static OpenTelemetryOptions AddOpenTelemetryBase(this IServiceCollection services, IConfiguration configuration)
    {
        var section = configuration.GetSection(OpenTelemetryOptions.SectionName);
        services.Configure<OpenTelemetryOptions>(section);

        return section.Get<OpenTelemetryOptions>() ?? new();
    }
}
