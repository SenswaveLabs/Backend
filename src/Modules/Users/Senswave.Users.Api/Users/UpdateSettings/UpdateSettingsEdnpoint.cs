namespace Senswave.Users.Api.Users.UpdateSettings;

internal sealed class UpdateSettingsEndpoint : IEndpoint
{
    public void Map(IEndpointRouteBuilder app)
    {
        app.MapPatch("users/settings", UpdateSettings)
            .MapToApiVersion(1)
            .WithTags(UsersModule.UserTag)
            .WithGroupName(UsersModule.GroupName)
            .Produces(204)
            .Produces<ErrorProblemDetails>(400)
            .Produces<ErrorProblemDetails>(404)
            .Produces(401);
    }

    [Authorize(AuthenticationSchemes = "Identity.Bearer")]
    private async Task<IResult> UpdateSettings([FromBody] UpdateSettingsRequest request, IMediator mediator, IRequestContext context)
    {
        var command = request.ToCommand(context.UserId);

        var result = await mediator.Send(command);

        if (result.IsFailure)
            return result.ToResultsDetails();

        return Results.NoContent();
    }
}

