using Microsoft.AspNetCore.Http;
using Senswave.Abstractions.Contexts;
using Senswave.Infrastructure.Web.Endpoints;
using Senswave.Users.Domain.Interfaces;
using System.Text.RegularExpressions;

namespace Senswave.Users.Infrastructure.Middlewares;

public class LegalMiddleware(
    IRequestContext requestContext,
    IUserService legalService,
    ILogger<LegalMiddleware> logger) : IMiddleware
{
    private static readonly Regex ApiPathToCheck =
        new(@"^/api/v\d+/(?!users|auth|legal)[^/]+(?:/.*)?$", RegexOptions.IgnoreCase | RegexOptions.Compiled, TimeSpan.FromMilliseconds(100));

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        if (requestContext.UserId != Guid.Empty && ApiPathToCheck.IsMatch(context.Request.Path.Value ?? string.Empty))
        {
            var hasConsents = await legalService.UserHasLatestConsents(requestContext.UserId, context.RequestAborted);

            if (!hasConsents.IsSuccess)
            {
                logger.LogWarning("[UserId: {UserId}] User does not have latest consents. Blocking request.",
                    requestContext.UserId);
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                context.Response.ContentType = "application/problem+json";

                var problem = new ErrorProblemDetails
                {
                    StatusCode = StatusCodes.Status403Forbidden,
                    Title = "User does not have latest consents.",
                    Type = "https://senswave.com/docs/errors/forbidden/consents",
                    Errors = hasConsents.Errors.ToList(),
                    TraceId = context.TraceIdentifier
                };

                await context.Response.WriteAsJsonAsync(problem, context.RequestAborted);
                return;
            }
        }

        await next(context);
    }
}
