namespace Senswave.Homes.Api.Homes.Features.DeleteHome;

internal sealed class DeleteHomeEndpoint : IEndpoint
{
    public void Map(IEndpointRouteBuilder app)
    {
        app.MapDelete("homes/{homeId:guid}", DeleteHome)
            .MapToApiVersion(1)
            .WithTags(HomesModule.HomesTag)
            .WithGroupName(HomesModule.GroupName)
            .Produces(204)
            .Produces<ErrorProblemDetails>(404);
    }

    [Authorize(AuthenticationSchemes = "Identity.Bearer")]
    private async Task<IResult> DeleteHome([FromRoute] Guid homeId, IMediator mediator, IRequestContext context)
    {
        var command = homeId.ToDeleteHomeCommand(context.UserId);
        var result = await mediator.Send(command);

        if (result.IsFailure)
            return result.ToResultsDetails();

        return Results.NoContent();
    }
}

