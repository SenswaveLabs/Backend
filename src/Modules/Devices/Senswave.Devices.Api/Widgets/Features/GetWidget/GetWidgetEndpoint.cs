using Senswave.Devices.Application.Widgets.Features.GetWidget;

namespace Senswave.Devices.Api.Widgets.Features.GetWidget;

internal sealed class GetWidgetEndpoint : IEndpoint
{
    public void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("devices/widgets/{widgetId:guid}", GetWidget)
            .MapToApiVersion(1)
            .WithTags(DevicesModule.WidgetsTag)
            .WithGroupName(DevicesModule.GroupName)
            .Produces<GetWidgetResponse>(200)
            .ProducesProblem(401)
            .Produces<ErrorProblemDetails>(404)
            .Produces<ErrorProblemDetails>(400);
    }

    [Authorize(AuthenticationSchemes = "Identity.Bearer")]
    private async Task<IResult> GetWidget([FromRoute] Guid widgetId, IMediator mediator, IRequestContext context)
    {
        var query = new GetWidgetQuery
        {
            UserId = context.UserId,
            WidgetId = widgetId
        };

        var result = await mediator.Send(query);

        if (result.IsFailure)
            return result.ToResultsDetails();

        var response = result.ToResponse();
        return Results.Ok(response);
    }
}

