using Senswave.Devices.Application.Dashboards.Features.DeleteDashboardWidget;

namespace Senswave.Devices.Api.Dashboards.DeleteDashboardWidget;

public class DeleteDashboardWidgetEndpoint : IEndpoint
{
    public void Map(IEndpointRouteBuilder app)
    {
        app.MapDelete("devices/dashboards/{dashboardId}/widgets/{widgetId}", DeleteDashboardWidget)
            .MapToApiVersion(1)
            .WithTags(DevicesModule.DashboardsTag)
            .WithGroupName(DevicesModule.GroupName)
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ErrorProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<ErrorProblemDetails>(StatusCodes.Status404NotFound);
    }

    [Authorize(AuthenticationSchemes = "Identity.Bearer")]
    private async Task<IResult> DeleteDashboardWidget(
        [FromRoute] Guid dashboardId,
        [FromRoute] Guid widgetId,
        IMediator mediator,
        IRequestContext context)
    {
        var command = new DeleteDashboardWidgetCommand
        {
            UserId = context.UserId,
            DashboardId = dashboardId,
            WidgetId = widgetId
        };

        var response = await mediator.Send(command);

        if (response.IsFailure)
            return response.ToResultsDetails();

        return Results.NoContent();
    }
}
