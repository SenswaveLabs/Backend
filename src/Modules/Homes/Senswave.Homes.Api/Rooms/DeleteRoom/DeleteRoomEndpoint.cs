namespace Senswave.Homes.Api.Rooms.DeleteRoom;

internal class DeleteRoomEndpoint : IEndpoint
{
    public void Map(IEndpointRouteBuilder app)
    {
        app.MapDelete("homes/{homeId:guid}/rooms/{roomId:guid}", DeleteRoom)
            .MapToApiVersion(1)
            .WithTags(HomesModule.RoomsTag)
            .WithGroupName(HomesModule.GroupName)
            .WithDescription("Delete a room from a home.")
            .Produces(204)
            .Produces<ErrorProblemDetails>(404)
            .Produces<ErrorProblemDetails>(400);
    }

    [Authorize(AuthenticationSchemes = "Identity.Bearer")]
    public async Task<IResult> DeleteRoom([FromRoute] Guid homeId, [FromRoute] Guid roomId, IMediator mediator, IRequestContext context)
    {
        var command = roomId.ToDeleteRoomCommand(homeId, context);

        var result = await mediator.Send(command);

        if (result.IsFailure)
            return result.ToResultsDetails();

        return Results.NoContent();
    }
}
