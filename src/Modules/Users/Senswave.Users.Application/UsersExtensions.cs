using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Senswave.Users.Application.Auth;
using Senswave.Users.Application.Auth.Google.Services;
using Senswave.Users.Domain;
using Senswave.Users.Domain.Diagnostics;

namespace Senswave.Users.Application;

public static class UsersExtensions
{
    public static IServiceCollection AddUsersApplication(this IServiceCollection services, IConfiguration configuration)
    {
        var assembly = typeof(UsersExtensions).Assembly;
        services.AddMediatR(config => config.RegisterServicesFromAssembly(assembly));
        services.AddValidatorsFromAssembly(assembly);

        services.Configure<UsersOptions>(configuration.GetSection(UsersOptions.SectionName));
        services.Configure<AuthOptions>(configuration.GetSection(AuthOptions.SectionName));

        services.AddScoped<IGoogleService, GoogleService>();
        services.AddSingleton<IUsersActivityProvider, UsersActivityProvider>();

        return services;
    }
}