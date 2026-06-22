using Microsoft.Extensions.DependencyInjection;
using Senswave.Abstractions.Contexts;
using Senswave.Abstractions.Cryptography;
using Senswave.Infrastructure.Web.Contexts;
using Senswave.Infrastructure.Web.Cryptography;

namespace Senswave.Infrastructure.Web;

public static class WebInfrastructureExtensions
{
    public static IServiceCollection AddWebInfrastructure(this IServiceCollection services)
    {
        services.AddProblemDetails();

        services.AddScoped<IPasswordHashingService, SimplePasswordHasherService>();

        // Http Context
        services.AddHttpContextAccessor();
        services.AddScoped<IRequestContext, UserRequestContext>();

        return services;
    }
}
