namespace Senswave.Devices.Api.Widgets.Features.State;

internal class StateEndpoint : IEndpoint
{
    public void Map(IEndpointRouteBuilder app)
    {
        app.MapPut("devices/widgets/{widgetId:guid}/state", StateAction)
            .MapToApiVersion(1)
            .WithTags(DevicesModule.WidgetsTag)
            .WithGroupName(DevicesModule.GroupName)
            .Produces(204)
            .ProducesProblem(401)
            .Produces<ErrorProblemDetails>(404)
            .Produces<ErrorProblemDetails>(400);
    }


    [Authorize(AuthenticationSchemes = "Identity.Bearer")]
    private async Task<IResult> StateAction(
        [FromBody] StateRequest request,
        [FromRoute] Guid widgetId,
        IMediator mediator,
        IRequestContext context)
    {
        var command = request.ToCommand(context, widgetId);

        var result = await mediator.Send(command);

        if (result.IsFailure)
            return result.ToResultsDetails();

        return Results.NoContent();
    }
}
