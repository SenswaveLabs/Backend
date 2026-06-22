using Senswave.Homes.Application.Rooms.Features.GetRoom;

namespace Senswave.Homes.Api.Rooms.GetRoom;

internal class GetRoomEndpoint : IEndpoint
{
    public void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("homes/{homeId:guid}/rooms/{roomId:guid}", GetRooms)
            .MapToApiVersion(1)
            .WithTags(HomesModule.RoomsTag)
            .WithGroupName(HomesModule.GroupName)
            .Produces<GetRoomResponse>(200)
            .Produces(401)
            .Produces<ErrorProblemDetails>(400)
            .Produces<ErrorProblemDetails>(404);
    }

    [Authorize(AuthenticationSchemes = "Identity.Bearer")]
    private async Task<IResult> GetRooms(
        [FromRoute] Guid homeId,
        [FromRoute] Guid roomId,
        IMediator mediator,
        IRequestContext context)
    {
        var query = new GetRoomQuery
        {
            UserId = context.UserId,
            RoomId = roomId,
            HomeId = homeId
        };

        var result = await mediator.Send(query);

        if (result.IsFailure)
            return result.ToResultsDetails();

        var response = result.Data
            .ToResponse();

        return Results.Ok(response);
    }
}
