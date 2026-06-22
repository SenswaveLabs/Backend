using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Senswave.Abstractions.Modules;
using Senswave.Automations.Application;
using Senswave.Automations.Domain;
using Senswave.Automations.Infrastructure;

namespace Senswave.Automations.Api;

public class AutomationsModule : ISenswaveModule
{
    public const string ModuleName = "Automations";

    public const string AutomationsTag = $"{ModuleName} - Automations";

    public static string GroupName => ModuleName.ToLower();

    public string Name => "Automations";

    public void Register(IServiceCollection services, IConfiguration configuration)
    {
        var assembly = typeof(AutomationsModule).Assembly;

        services.AddAutomationsDomain(configuration);
        services.AddAutomationsApplication();
        services.AddAutomationsInfrastructure();
        services.AddMinimalApiEndpoints(assembly);
    }
}
