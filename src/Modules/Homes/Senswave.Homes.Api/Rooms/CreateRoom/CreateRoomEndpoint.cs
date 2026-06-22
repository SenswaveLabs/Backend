namespace Senswave.Homes.Api.Rooms.CreateRoom;

internal sealed class CreateRoomEndpoint : IEndpoint
{
    public void Map(IEndpointRouteBuilder app)
    {
        app.MapPost("homes/{homeId:guid}/rooms", CreateRoom)
            .MapToApiVersion(1)
            .WithTags(HomesModule.RoomsTag)
            .WithGroupName(HomesModule.GroupName)
            .Produces(201)
            .Produces(401)
            .Produces<ErrorProblemDetails>(400)
            .Produces<ErrorProblemDetails>(404)
            .Produces<ErrorProblemDetails>(409);
    }

    [Authorize(AuthenticationSchemes = "Identity.Bearer")]
    private async Task<IResult> CreateRoom([FromBody] CreateRoomRequest request, [FromRoute] Guid homeId, IMediator mediator, IRequestContext context)
    {
        var command = request.ToCommand(context.UserId, homeId);
        var result = await mediator.Send(command);

        if (result.IsFailure)
            return result.ToResultsDetails();

        var response = result.ToCreatedResponse();
        return Results.Created($"/{result.Data.Id}", response);
    }
}
