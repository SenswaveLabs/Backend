using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Senswave.Devices.Domain.Dashboards.Factories;
using Senswave.Devices.Domain.Devices.Factory;
using Senswave.Devices.Domain.Devices.Options;
using Senswave.Devices.Domain.Operations.Factory;
using Senswave.Devices.Domain.Widgets.Factory;

namespace Senswave.Devices.Domain;

public static class DevicesExtensions
{
    public static IServiceCollection AddDevicesDomain(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<DevicesOptions>(configuration.GetSection(DevicesOptions.SectionName));

        services.AddScoped<OperationFactory>();
        services.AddScoped<DashboardFactory>();
        services.AddScoped<WidgetFactory>();
        services.AddScoped<DeviceTileFactory>();
        services.AddScoped<DeviceFactory>();

        return services;
    }
}
