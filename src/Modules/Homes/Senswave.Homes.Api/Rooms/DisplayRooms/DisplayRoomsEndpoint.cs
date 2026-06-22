using Senswave.Homes.Application.Rooms.Features.DisplayRooms;

namespace Senswave.Homes.Api.Rooms.DisplayRooms;

internal sealed class GetRoomsEndpoint : IEndpoint
{
    public void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("homes/{homeId:guid}/rooms/display", GetRooms)
            .MapToApiVersion(1)
            .WithTags(HomesModule.RoomsTag)
            .WithGroupName(HomesModule.GroupName)
            .Produces<DisplayRoomsResponse>(200)
            .Produces(401)
            .Produces<ErrorProblemDetails>(400)
            .Produces<ErrorProblemDetails>(404)
            .Produces<ErrorProblemDetails>(409);
    }

    [Authorize(AuthenticationSchemes = "Identity.Bearer")]
    private async Task<IResult> GetRooms([FromRoute] Guid homeId, IMediator mediator, IRequestContext context)
    {
        var query = new DisplayRoomsQuery
        {
            UserId = context.UserId,
            HomeId = homeId
        };

        var result = await mediator.Send(query);

        if (result.IsFailure)
            return result.ToResultsDetails();

        var response = result.ToResponse();
        return Results.Ok(response);
    }
}

