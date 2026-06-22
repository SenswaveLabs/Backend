namespace Senswave.Homes.Api.Homes.Features.UpdateHome;

internal sealed class UpdateHomeEndpoint : IEndpoint
{
    public void Map(IEndpointRouteBuilder app)
    {
        app.MapPatch("homes/{homeId:guid}", UpdateHome)
            .MapToApiVersion(1)
            .WithTags(HomesModule.HomesTag)
            .WithGroupName(HomesModule.GroupName)
            .Produces(204)
            .Produces(401)
            .Produces<ErrorProblemDetails>(400)
            .Produces<ErrorProblemDetails>(404);
    }

    [Authorize(AuthenticationSchemes = "Identity.Bearer")]
    private async Task<IResult> UpdateHome([FromRoute] Guid homeId, [FromBody] UpdateHomeRequest request, IMediator mediator, IRequestContext context)
    {
        var command = request.ToCommand(context.UserId, homeId);
        var result = await mediator.Send(command);

        if (result.IsFailure)
            return result.ToResultsDetails();

        return Results.NoContent();
    }
}

