using Senswave.Users.Application.Users.GetUser;

namespace Senswave.Users.Api.Users.GetUser;

internal sealed class GetUserEndpoint : IEndpoint
{
    public void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("users", GetSettings)
            .MapToApiVersion(1)
            .WithTags(UsersModule.UserTag)
            .WithGroupName(UsersModule.GroupName)
            .Produces<GetUserResponse>(200)
            .Produces(401)
            .Produces<ErrorProblemDetails>(404);
    }

    [Authorize(AuthenticationSchemes = "Identity.Bearer")]
    private async Task<IResult> GetSettings(IMediator mediator, IRequestContext context)
    {
        var query = new GetUserQuery
        {
            UserId = context.UserId
        };

        var result = await mediator.Send(query);

        if (result.IsFailure)
            return result.ToResultsDetails();

        var dto = result.Data.ToResponse();
        return Results.Ok(dto);
    }
}

