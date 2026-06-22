using Microsoft.Extensions.DependencyInjection;
using Senswave.Automations.Application.Services;
using Senswave.Automations.Domain.Services;

namespace Senswave.Automations.Application;

public static class AutomationsExtensions
{
    public static IServiceCollection AddAutomationsApplication(this IServiceCollection services)
    {
        var assembly = typeof(AutomationsExtensions).Assembly;

        services.AddTransient<IAutomationInterpreter, AutomationInterpreter>();
        services.AddTransient<IAutomationAccessService, AutomationAccessService>();
        services.AddTransient<IAutomationActionService, AutomationActionService>();

        services.AddMediatR(config => config.RegisterServicesFromAssembly(assembly));
        services.AddValidatorsFromAssembly(assembly);

        return services;
    }
}
