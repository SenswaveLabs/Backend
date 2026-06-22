using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Senswave.Abstractions.Modules;
using Senswave.Homes.Application;
using Senswave.Homes.Domain;
using Senswave.Homes.Infrastructure;

namespace Senswave.Homes.Api;

public class HomesModule : ISenswaveModule
{
    public const string ModuleName = "Homes";

    public const string HomesTag = $"{ModuleName} - Homes";
    public const string RoomsTag = $"{ModuleName} - Rooms";
    public const string SharingTag = $"{ModuleName} - Sharing";

    public static string GroupName => ModuleName.ToLower();

    public string Name => ModuleName;

    public void Register(IServiceCollection services, IConfiguration configuration)
    {
        var assembly = typeof(HomesModule).Assembly;

        services.AddHomesDomain(configuration);
        services.AddHomesApplication();
        services.AddHomesInfrastructure();
        services.AddMinimalApiEndpoints(assembly);
    }
}