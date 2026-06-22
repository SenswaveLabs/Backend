using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Senswave.Infrastructure.Behaviors;
using Senswave.Infrastructure.Diagnostics;
using Senswave.Infrastructure.Messaging.Extensions;
using Senswave.Infrastructure.Persistence;

namespace Senswave.Infrastructure;

public static class InfrastructureModule
{
    public const string DefaultListenerName = "Infrastructure";

    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Diagnostics
        services.AddSingleton<IInfrastructureDiagnosticsActivityProvider, InfrastructureDiagnosticsActivityProvider>();

        // Messaging
        services.AddMessaging(configuration);

        // Database
        services.Configure<PostgreSqlOptions>(configuration.GetSection(PostgreSqlOptions.SectionName));
        services.AddHealthChecks()
            .AddCheck<PostgresHealthCheck>("postgres");

        // Cache
        services.AddDistributedMemoryCache();

        // Http
        services.AddOptions();

        services.AddMediatR(config => config.RegisterServicesFromAssembly(typeof(LoggingBehaviour<,>).Assembly));
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(LoggingBehaviour<,>));
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));

        return services;
    }

    public static IServiceCollection AddSimpleInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Diagnostics
        services.AddSingleton<IInfrastructureDiagnosticsActivityProvider, InfrastructureDiagnosticsActivityProvider>();

        // Messaging
        services.AddMessaging(configuration);

        // Cache
        services.AddDistributedMemoryCache();

        // Http
        services.AddOptions();

        return services;
    }
}
