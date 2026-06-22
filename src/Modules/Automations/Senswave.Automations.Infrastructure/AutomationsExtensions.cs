using MassTransit.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Senswave.Automations.Domain.Repositories;
using Senswave.Automations.Infrastructure.Consumers;
using Senswave.Automations.Infrastructure.Repositories;

namespace Senswave.Automations.Infrastructure;

public static class AutomationsExtensions
{
    public static IServiceCollection AddAutomationsInfrastructure(this IServiceCollection services)
    {
        services.AddDbContext<AutomationsContext>();

        services.AddScoped<IQueryAutomationRepository, QueryAutomationRepository>();
        services.AddScoped<ICommandAutomationRepository, CommandAutomationRepository>();
        services.AddScoped<ICommandResultRepository, CommandResultRepository>();
        services.AddScoped<ICommandConditionRepository, CommandConditionRepository>();

        services.RegisterConsumer<AutomationTriggerConsumer>();
        services.RegisterConsumer<AutomationsRemovalConsumer>();

        return services;
    }
}
