using MassTransit.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Senswave.Homes.Domain.Homes.Features.GetCurrentHome;
using Senswave.Homes.Domain.Homes.Repositories;
using Senswave.Homes.Domain.Rooms.Repositories;
using Senswave.Homes.Domain.Sharings.Repositories;
using Senswave.Homes.Infrastructure.Consumers;
using Senswave.Homes.Infrastructure.Homes.Consumers;
using Senswave.Homes.Infrastructure.Homes.Features.GetCurrentHome;
using Senswave.Homes.Infrastructure.Homes.Repositories;
using Senswave.Homes.Infrastructure.Rooms.Consumers;
using Senswave.Homes.Infrastructure.Rooms.Features.GetRooms;
using Senswave.Homes.Infrastructure.Rooms.Repositories;
using Senswave.Homes.Infrastructure.Sharings.Consumers;
using Senswave.Homes.Infrastructure.Sharings.Repositories;

namespace Senswave.Homes.Infrastructure;

public static class HomesExtensions
{
    public static IServiceCollection AddHomesInfrastructure(this IServiceCollection services)
    {
        services.AddDbContext<HomesContext>();

        services.AddScoped<IHomeQueryRepository, HomeQueryRepository>();
        services.AddScoped<IHomeCommandRepository, HomeCommandRepository>();
        services.AddScoped<IGetCurrentHomeRepository, GetCurrentHomeRepository>();

        services.AddScoped<IHomeSharingCommandRepository, HomeSharingCommandRepository>();
        services.AddScoped<IHomeSharingQueryRepository, HomeSharingQueryRepository>();

        services.AddScoped<IRoomCommandRepository, RoomCommandRepository>();
        services.AddScoped<IRoomQueryRepository, RoomQueryRepository>();

        services.RegisterConsumer<HomeRemovalConsumer>();
        services.RegisterConsumer<HomeConsumer>();
        services.RegisterConsumer<HomeUsersConsumer>();
        services.RegisterConsumer<HomeSharedConsumer>();
        services.RegisterConsumer<HomeAccessConsumer>();
        services.RegisterConsumer<HomeDataSourceConsumer>();
        services.RegisterConsumer<CanDisplayHomeConsumer>();
        services.RegisterConsumer<CanManageHomeConsumer>();
        services.RegisterConsumer<IsHomeOwnerConsumer>();

        services.RegisterConsumer<RoomAccessConsumer>();
        services.RegisterConsumer<BrokerUsageConsumer>();

        return services;
    }
}