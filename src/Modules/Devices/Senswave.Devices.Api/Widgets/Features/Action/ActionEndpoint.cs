namespace Senswave.Devices.Api.Widgets.Features.Action;

internal sealed class ActionEndpoint : IEndpoint
{
    public void Map(IEndpointRouteBuilder app)
    {
        app.MapPost("devices/widgets/{widgetId:guid}/action", Action)
            .MapToApiVersion(1)
            .WithTags(DevicesModule.WidgetsTag)
            .WithGroupName(DevicesModule.GroupName)
            .Produces(204)
            .ProducesProblem(401)
            .Produces<ErrorProblemDetails>(404)
            .Produces<ErrorProblemDetails>(400);
    }

    [Authorize(AuthenticationSchemes = "Identity.Bearer")]
    private async Task<IResult> Action([FromBody] WidgetActionRequest dto, [FromRoute] Guid widgetId, IMediator mediator, IRequestContext context)
    {
        var command = dto.ToCommand(context.UserId, widgetId);

        var result = await mediator.Send(command);

        if (result.IsFailure)
            return result.ToResultsDetails();

        return Results.NoContent();
    }
}
