using Microsoft.Extensions.DependencyInjection;
using Senswave.Devices.Application.Dashboards.Services;
using Senswave.Devices.Application.Devices.Services;
using Senswave.Devices.Application.Operations.Services;
using Senswave.Devices.Application.Services;
using Senswave.Devices.Application.Widgets.Services;
using Senswave.Devices.Domain.Dashboards.Services;
using Senswave.Devices.Domain.Devices.Services;
using Senswave.Devices.Domain.Operations.Services;
using Senswave.Devices.Domain.Services;
using Senswave.Devices.Domain.Widgets.Services;

namespace Senswave.Devices.Application;

public static class DevicesExtensions
{
    public static IServiceCollection AddDevicesApplication(this IServiceCollection services)
    {
        var assembly = typeof(DevicesExtensions).Assembly;
        services.AddMediatR(config => config.RegisterServicesFromAssembly(assembly));
        services.AddValidatorsFromAssembly(assembly);

        services.AddScoped<IOperationAccessService, OperationAccessService>();
        services.AddScoped<IOperationActionService, OperationActionService>();

        services.AddScoped<IDeviceService, DeviceService>();
        services.AddScoped<IDeviceAccessService, DeviceAccessService>();

        services.AddScoped<IDashboardAccessService, DashboardAccessService>();

        services.AddScoped<IWidgetAccessService, WidgetAccessService>();
        services.AddScoped<IWidgetService, WidgetService>();

        services.AddScoped<IActionService, ActionService>();

        return services;
    }
}
