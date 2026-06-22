using Senswave.Users.Application.Users.DeleteAccount;

namespace Senswave.Users.Api.Users.DeleteAccount;

internal class DeleteAccountEndpoint : IEndpoint
{
    public void Map(IEndpointRouteBuilder app)
    {
        app.MapDelete("users/account", DeleteAccountAsync)
            .MapToApiVersion(1)
            .WithTags(UsersModule.UserTag)
            .WithGroupName(UsersModule.GroupName)
            .Produces(204)
            .Produces<ErrorProblemDetails>(500);
    }

    [Authorize(AuthenticationSchemes = "Identity.Bearer")]
    private async Task<IResult> DeleteAccountAsync(IMediator mediator, IRequestContext context)
    {
        var command = new DeleteAccountCommand
        {
            UserId = context.UserId
        };

        var result = await mediator.Send(command);

        if (result.IsFailure)
            return result.ToResultsDetails();

        return Results.NoContent();
    }
}
