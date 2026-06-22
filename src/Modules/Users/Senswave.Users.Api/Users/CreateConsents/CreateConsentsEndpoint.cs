using Senswave.Users.Application.Users.CreateConsents;

namespace Senswave.Users.Api.Users.CreateConsents;

public class CreateConsentsEndpoint : IEndpoint
{
    public void Map(IEndpointRouteBuilder app)
    {
        app.MapPost("users/consents", CreateConsents)
            .MapToApiVersion(1)
            .WithTags(UsersModule.UserTag)
            .WithGroupName(UsersModule.GroupName)
            .Produces(204)
            .Produces<ErrorProblemDetails>(500);
    }

    [Authorize(AuthenticationSchemes = "Identity.Bearer")]
    private async Task<IResult> CreateConsents(IMediator mediator, IRequestContext context)
    {
        var query = new CreateConsentsCommand
        {
            UserId = context.UserId
        };

        var result = await mediator.Send(query);

        if (result.IsFailure)
            return result.ToResultsDetails();

        return Results.NoContent();
    }
}
