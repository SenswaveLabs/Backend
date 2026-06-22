namespace Senswave.Homes.Api.Sharings.LeaveSharing;

internal class LeaveSharingEndpoint : IEndpoint
{
    public void Map(IEndpointRouteBuilder app)
    {
        app.MapDelete("homes/sharings/leave/{homeId}", LeaveSharing)
            .MapToApiVersion(1)
            .WithTags(HomesModule.SharingTag)
            .WithGroupName(HomesModule.GroupName)
            .WithDescription("Allows user to leave home that he was invited to.")
            .Produces(204)
            .Produces<ErrorProblemDetails>(400)
            .Produces<ErrorProblemDetails>(500);
    }

    [Authorize(AuthenticationSchemes = "Identity.Bearer")]
    private async Task<IResult> LeaveSharing([FromRoute] Guid homeId, IMediator mediator, IRequestContext context)
    {
        var command = homeId.ToLeaveSharingCommand(context);

        var results = await mediator.Send(command);

        if (results.IsFailure)
            return results.ToResultsDetails();

        return Results.NoContent();
    }
}
