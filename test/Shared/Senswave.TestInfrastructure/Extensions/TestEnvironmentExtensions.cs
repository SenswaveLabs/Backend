using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Senswave.Infrastructure.Messaging.Options;
using Senswave.Infrastructure.Persistence;
using Senswave.TestInfrastructure.Fixtures.Database;
using Senswave.TestInfrastructure.Fixtures.MessageBus;
using Senswave.Users.Application.Auth.Google.Services;

namespace Senswave.TestInfrastructure.Extensions;

public static partial class TestEnvironmentExtensions
{
    public static void ReplaceBusToInMemory(this IServiceCollection services)
    {
        services.ReplaceOptions<MessageBusOptions>(new MessageBusOptions { Type = "InMemory" });
    }

    public static void ReplaceRabbitMqBus(this IServiceCollection services, IMessageBus messageBus)
    {
        services.AddMassTransitTestHarness(bus =>
        {
            bus.SetKebabCaseEndpointNameFormatter();

            bus.UsingRabbitMq((context, config) =>
            {
                config.Host(new Uri(messageBus.GetConnectionString()), host =>
                {
                    host.Username(messageBus.GetUsername());
                    host.Password(messageBus.GetPassword());
                });
                config.ConfigureEndpoints(context);
            });
        });
    }

    public static void ReplaceGoogleServices(this IServiceCollection services, IGoogleService service)
    {
        services.ReplaceService(service);
    }

    public static void ReplacePostgreSql(this IServiceCollection services, IDatabase database)
    {
        services.Configure<PostgreSqlOptions>(options =>
        {
            options.Port = database.GetPort().ToString();
            options.Host = database.GetHostname();
            options.Username = database.GetUsername();
            options.Password = database.GetPassword();
            options.Database = database.GetDatabase();
        });
    }

    public static void ReplaceOptions<T>(this IServiceCollection services, T options) where T : class, new()
    {
        var serviceDescriptor = services.FirstOrDefault(descriptor => descriptor.GetType() == typeof(IOptions<T>));
        var snapshotServiceDescriptor = services.FirstOrDefault(descriptor => descriptor.GetType() == typeof(IOptionsSnapshot<T>));

        if (serviceDescriptor != null)
            services.Remove(serviceDescriptor);

        if (snapshotServiceDescriptor != null)
            services.Remove(snapshotServiceDescriptor);

        services.AddSingleton(Options.Create(options));
        services.AddSingleton<IOptionsSnapshot<T>>(sp => new StaticOptionsSnapshot<T>(options));
    }

    public static void ReplaceService<T>(this IServiceCollection services, T service) where T : class
    {
        var serviceDescriptor = services.FirstOrDefault(descriptor => descriptor.GetType() == typeof(T));

        if (serviceDescriptor != null)
            services.Remove(serviceDescriptor);

        services.AddSingleton<T>(service);
    }
}
