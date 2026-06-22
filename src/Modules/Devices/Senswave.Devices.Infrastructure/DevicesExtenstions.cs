using MassTransit.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Senswave.Devices.Domain.Dashboards.Repositories;
using Senswave.Devices.Domain.Devices.Repositories;
using Senswave.Devices.Domain.Operations.Repositories;
using Senswave.Devices.Domain.ShareDevices.Repositories;
using Senswave.Devices.Domain.Widgets.Repositories;
using Senswave.Devices.Infrastructure.Consumers;
using Senswave.Devices.Infrastructure.Dashboards.Repositories;
using Senswave.Devices.Infrastructure.Devices.Consumers;
using Senswave.Devices.Infrastructure.Devices.Repositories;
using Senswave.Devices.Infrastructure.Operations.Consumers;
using Senswave.Devices.Infrastructure.Operations.Repositories;
using Senswave.Devices.Infrastructure.ShareDevices.Consumers;
using Senswave.Devices.Infrastructure.ShareDevices.Repositories;
using Senswave.Devices.Infrastructure.Widgets.Repositories;

namespace Senswave.Devices.Infrastructure;

public static class DevicesExtenstions
{
    public static IServiceCollection AddDevicesInfrastructure(this IServiceCollection services)
    {
        services.AddDbContext<DevicesContext>();

        services.AddScoped<IDashboardsQueryRespository, DashboardsQueryRepository>();
        services.AddScoped<IDashboardCommandRepository, DashboardCommandRepository>();

        services.AddScoped<IDeviceQueryRepository, DevicesQueryRepository>();
        services.AddScoped<IDeviceCommandRepository, DevicesCommandRepository>();

        services.AddScoped<IWidgetCommandRepository, WidgetCommandRepository>();
        services.AddScoped<IWidgetQueryRepository, WidgetQueryRepository>();

        services.AddScoped<IOperationCommandRepository, OperationCommandRepository>();
        services.AddScoped<IOperationQueryRepository, OperationRepository>();
        services.AddScoped<IOperationCleanupRepository, OperationValueClenaupRepository>();

        services.AddScoped<IDeviceSharingCommandRepository, DeviceSharingCommandRepository>();
        services.AddScoped<IDeviceSharingQueryRepository, DeviceSharingQueryRepository>();

        services.RegisterConsumer<DevicesRemovalConsumer>();
        services.RegisterConsumer<LastOperationValueConsumer>();
        services.RegisterConsumer<MessageForProcessingConsumer>();
        services.RegisterConsumer<DevicesInHomeConsumer>();
        services.RegisterConsumer<ActionAccessToOperationsConsumer>();
        services.RegisterConsumer<DevicesConsumer>();
        services.RegisterConsumer<OperationNameByIdConsumer>();
        services.RegisterConsumer<ExternalDeviceActionConsumer>();
        services.RegisterConsumer<RemoveDeviceSharingConsumer>();
        services.RegisterConsumer<OperationExists>();
        services.RegisterConsumer<SubscriptionUsageConsumer>();

        return services;
    }
}
