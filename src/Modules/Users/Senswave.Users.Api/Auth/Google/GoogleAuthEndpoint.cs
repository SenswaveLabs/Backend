using Microsoft.AspNetCore.Authentication.BearerToken;
using Senswave.Users.Application.Auth.Google.Login;

namespace Senswave.Users.Api.Auth.Google;

public class GoogleAuthEndpoint : IEndpoint
{
    public void Map(IEndpointRouteBuilder app)
    {
        app.MapPost("auth/login/google", Login)
            .MapToApiVersion(1)
            .Produces<AccessTokenResponse>(200)
            .Produces<ErrorProblemDetails>(400)
            .WithTags(UsersModule.AuthorizationTag)
            .WithGroupName(UsersModule.GroupName);
    }

    private async Task<IResult> Login(
        IMediator mediator,
        [FromBody] GoogleAuthRequest request)
    {
        var command = new GoogleAuthCommand
        {
            Code = request.Code
        };

        var result = await mediator.Send(command);

        if (result.IsFailure)
            return result.ToResultsDetails();

        return TypedResults.Empty;
    }
}
