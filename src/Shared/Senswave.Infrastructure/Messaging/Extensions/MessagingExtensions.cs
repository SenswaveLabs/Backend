using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Senswave.Abstractions.Messaging.Enums;
using Senswave.Abstractions.Messaging.Interfaces;
using Senswave.Infrastructure.Messaging.Implementations;
using Senswave.Infrastructure.Messaging.Options;

namespace Senswave.Infrastructure.Messaging.Extensions;

internal static class MessagingExtensions
{
    internal static IServiceCollection AddMessaging(this IServiceCollection services, IConfiguration configuration)
    {
        var section = configuration.GetSection(MessageBusOptions.SectionName);
        var options = section.Get<MessageBusOptions>();

        services.Configure<MessageBusOptions>(section);
        services.Configure<RabbitMqOptions>(configuration.GetSection(RabbitMqOptions.SectionName));
        services.Configure<KafkaOptions>(configuration.GetSection(KafkaOptions.SectionName));

        services.AddMassTransit(bus =>
        {
            bus.SetKebabCaseEndpointNameFormatter();

            if (options!.ProviderType == MessagingProviderType.InMemory)
            {
                bus.UsingInMemory((context, config) =>
                {
                    config.ConfigureEndpoints(context);
                });
            }
            else if (options!.ProviderType == MessagingProviderType.RabbitMq)
            {
                bus.UsingRabbitMq((context, config) =>
                {
                    var options = context.GetRequiredService<IOptions<RabbitMqOptions>>().Value;

                    config.Host(new Uri(options.Host), host =>
                    {
                        host.Username(options.Username);
                        host.Password(options.Password);
                    });

                    config.ConfigureEndpoints(context);
                });
            }
            else
            {
                throw new ConfigurationException("Invalid message bus provider type");
            }
        });

        services.AddScoped<IPublishMessageBus, PublishMessageBus>();
        services.AddSingleton<IMessageBus, MessageBus>();

        return services;
    }
}
