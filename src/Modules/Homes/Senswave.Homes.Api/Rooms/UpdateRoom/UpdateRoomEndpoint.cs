namespace Senswave.Homes.Api.Rooms.UpdateRoom;

internal sealed class UpdateRoomEndpoint : IEndpoint
{
    public void Map(IEndpointRouteBuilder app)
    {
        app.MapPatch("homes/{homeId:guid}/rooms/{roomId:guid}", UpdateRoom)
            .MapToApiVersion(1)
            .WithTags(HomesModule.RoomsTag)
            .WithGroupName(HomesModule.GroupName)
            .Produces(204)
            .Produces(401)
            .Produces<ErrorProblemDetails>(400)
            .Produces<ErrorProblemDetails>(404)
            .Produces<ErrorProblemDetails>(409);
    }

    [Authorize(AuthenticationSchemes = "Identity.Bearer")]
    private async Task<IResult> UpdateRoom([FromBody] UpdateRoomRequest request, [FromRoute] Guid roomId, IMediator mediator, IRequestContext context)
    {
        var command = request.ToCommand(context.UserId, roomId);

        var result = await mediator.Send(command);

        if (result.IsFailure)
            return result.ToResultsDetails();

        return Results.NoContent();
    }
}

