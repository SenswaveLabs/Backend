namespace Senswave.Homes.Api.Sharings.AcceptSharing;

internal sealed class AcceptSharingEndpoint : IEndpoint
{
    public void Map(IEndpointRouteBuilder app)
    {
        app.MapPut("homes/sharings", AcceptSharing)
            .MapToApiVersion(1)
            .WithTags(HomesModule.SharingTag)
            .WithGroupName(HomesModule.GroupName)
            .Produces(204)
            .Produces(401)
            .Produces<ErrorProblemDetails>(400)
            .Produces<ErrorProblemDetails>(404);
    }

    [Authorize(AuthenticationSchemes = "Identity.Bearer")]
    private async Task<IResult> AcceptSharing([FromBody] AcceptSharingRequest request, IMediator mediator, IRequestContext context)
    {
        var command = request.ToCommand(context.UserId);
        var results = await mediator.Send(command);

        if (results.IsFailure)
            return results.ToResultsDetails();

        return Results.NoContent();
    }
}

