namespace Senswave.Homes.Api.Sharings.DeleteSharing;

internal sealed class DeleteSharingEndpoint : IEndpoint
{
    public void Map(IEndpointRouteBuilder app)
    {
        app.MapDelete("homes/sharings/{homeSharingId:guid}", DeleteSharing)
            .MapToApiVersion(1)
            .WithTags(HomesModule.SharingTag)
            .WithGroupName(HomesModule.GroupName)
            .Produces(204)
            .Produces<ErrorProblemDetails>(404);
    }

    [Authorize(AuthenticationSchemes = "Identity.Bearer")]
    private async Task<IResult> DeleteSharing([FromRoute] Guid homeSharingId, IMediator mediator, IRequestContext context)
    {
        var command = homeSharingId.ToDeleteSharingCommand(context.UserId);
        var results = await mediator.Send(command);

        if (results.IsFailure)
            return results.ToResultsDetails();

        return Results.NoContent();
    }
}

