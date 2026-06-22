namespace Senswave.Devices.Api.Widgets.Features.DeleteWidget;

internal sealed class DeleteWidgetEndpoint : IEndpoint
{
    public void Map(IEndpointRouteBuilder app)
    {
        app.MapDelete("devices/widgets/{widgetId:guid}", DeleteWidget)
            .MapToApiVersion(1)
            .WithTags(DevicesModule.WidgetsTag)
            .WithGroupName(DevicesModule.GroupName)
            .Produces(204)
            .ProducesProblem(401)
            .Produces<ErrorProblemDetails>(400)
            .Produces<ErrorProblemDetails>(404);
    }

    [Authorize(AuthenticationSchemes = "Identity.Bearer")]
    private async Task<IResult> DeleteWidget([FromRoute] Guid widgetId, IMediator mediator, IRequestContext context)
    {
        var command = widgetId.ToDeleteWidgetCommand(context.UserId);

        var result = await mediator.Send(command);

        if (result.IsFailure)
            return result.ToResultsDetails();

        return Results.NoContent();
    }
}

