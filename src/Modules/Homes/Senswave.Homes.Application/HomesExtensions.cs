using Microsoft.Extensions.DependencyInjection;
using Senswave.Homes.Application.Homes.Services;
using Senswave.Homes.Application.Rooms.Services;
using Senswave.Homes.Application.Sharings.Services;
using Senswave.Homes.Domain.Homes.Services;
using Senswave.Homes.Domain.Rooms.Services;
using Senswave.Homes.Domain.Sharings.Services;

namespace Senswave.Homes.Application;

public static class HomesExtensions
{
    public static IServiceCollection AddHomesApplication(this IServiceCollection services)
    {
        var assembly = typeof(HomesExtensions).Assembly;
        services.AddMediatR(config => config.RegisterServicesFromAssembly(assembly));
        services.AddValidatorsFromAssembly(assembly);

        services.AddScoped<IHomeAccessService, HomeAccessService>();

        services.AddScoped<IRoomAccessService, RoomAccessService>();

        services.AddScoped<ISharingPasswordGeneratorService, PasswordGeneratorService>();

        return services;
    }
}