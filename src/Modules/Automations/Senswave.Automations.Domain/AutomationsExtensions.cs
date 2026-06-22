using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Senswave.Automations.Domain.Factory;
using Senswave.Automations.Domain.Options;

namespace Senswave.Automations.Domain;

public static class AutomationsExtensions
{
    public static IServiceCollection AddAutomationsDomain(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<AutomationOptions>(configuration.GetSection(AutomationOptions.SectionName));

        services.AddScoped<ConditionFactory>();
        return services;
    }
}
