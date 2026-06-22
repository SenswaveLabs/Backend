using MassTransit.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Senswave.Abstractions.Modules;
using Senswave.LiveUpdates.Api.Consumers;
using Senswave.LiveUpdates.Api.Diagnostics;
using Senswave.LiveUpdates.Api.Services.DataSourcesUpdates;
using Senswave.LiveUpdates.Api.Services.DevicesUpdates;
using Senswave.LiveUpdates.Api.Services.RateLimiter;

namespace Senswave.LiveUpdates.Api;

public class LiveUpdatesModule : ISenswaveModule
{
    public const string DefaultListenerName = "Senswave.LiveUpdate";

    public const string InitializationTag = $"{ModuleName} - Initialization";
    public const string LiveUpdatesTag = $"{ModuleName} - Live Updates";

    public const string GroupName = "live update";
    public const string ModuleName = "Live Update";

    public string Name => ModuleName;

    public void Register(IServiceCollection services, IConfiguration configuration)
    {
        services.AddSignalR();

        services.RegisterConsumer<DeviceTileActionEventConsumer>();
        services.RegisterConsumer<WidgetActionEventConsumer>();
        services.RegisterConsumer<DataSourceStateConsumer>();
        services.RegisterConsumer<DevicePresenceEventConsumer>();

        services.AddScoped<IDevicesUpdateService, DevicesUpdateService>();
        services.AddScoped<IDataSourcesUpdateService, DataSourcesUpdateService>();

        services.AddSingleton<IRateLimiterService, RateLimitingService>();

        services.AddSingleton<ILiveUpdatesActivityProvider, LiveUpdatesActivityProvider>();

        services.Configure<LiveUpdatesOptions>(configuration.GetSection(LiveUpdatesOptions.SectionName));
    }
}
