using Microsoft.AspNetCore.Authentication.BearerToken;
using Senswave.Users.Application.Auth.Google.LoginV2;

namespace Senswave.Users.Api.Auth.GoogleV2;

public class GoogleJwtAuthEndpoint : IEndpoint
{
    public void Map(IEndpointRouteBuilder app)
    {
        app.MapPost("auth/login/google", Login)
            .MapToApiVersion(2)
            .Produces<AccessTokenResponse>(200)
            .Produces<ErrorProblemDetails>(400)
            .WithTags(UsersModule.AuthorizationTag)
            .WithGroupName(UsersModule.GroupName);
    }

    private async Task<IResult> Login(
        IMediator mediator,
        [FromBody] GoogleJwtAuthRequest request)
    {
        var command = new GoogleJwtAuthCommand
        {
            JwtToken = request.Token
        };

        var result = await mediator.Send(command);

        if (result.IsFailure)
            return result.ToResultsDetails();

        return TypedResults.Empty;
    }
}
