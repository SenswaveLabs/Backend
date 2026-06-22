using Senswave.Users.Domain.Entity;

namespace Senswave.Users.Api;

public static class UsersModuleExtensions
{
    public static IEndpointRouteBuilder UseAuthEndpoints(this IEndpointRouteBuilder endpoints, string rateLimiterPolicy)
    {
        endpoints.MapGroup("api/v1/auth")
            .WithTags(UsersModule.AuthorizationTag)
            .RequireRateLimiting(rateLimiterPolicy)
            .MapIdentityApi<User>();

        return endpoints;
    }
}
