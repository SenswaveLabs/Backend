using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace Senswave.Presentation.DataSource.Worker.Public;

public static class PublicModuleExtensions
{
    public static IServiceCollection AddPublicModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<VersionOptions>(configuration.GetSection(VersionOptions.SectionName));
        return services;
    }

    public static IEndpointRouteBuilder MapApiPublicEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("api/version", (IOptionsSnapshot<VersionOptions> options) =>
        {
            var versionInfo = new
            {
                version = options.Value.Version
            };

            return Results.Ok(versionInfo);
        }).WithTags(PublicModule.ModuleName)
          .WithName("Get API Version")
          .WithMetadata(new AllowAnonymousAttribute());

        return endpoints;
    }

}
