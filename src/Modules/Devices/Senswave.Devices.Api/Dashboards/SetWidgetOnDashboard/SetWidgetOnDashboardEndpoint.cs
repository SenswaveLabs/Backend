namespace Senswave.Devices.Api.Dashboards.SetWidgetOnDashboard;

public class SetWidgetOnDashboardEndpoint : IEndpoint
{
    public void Map(IEndpointRouteBuilder app)
    {
        app.MapPut("devices/dashboards/{dashboardId}/widgets", SetWidgetOnDashboard)
            .MapToApiVersion(1)
            .WithTags(DevicesModule.DashboardsTag)
            .WithGroupName(DevicesModule.GroupName)
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ErrorProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<ErrorProblemDetails>(StatusCodes.Status404NotFound)
            .Produces<ErrorProblemDetails>(StatusCodes.Status409Conflict);
    }

    [Authorize(AuthenticationSchemes = "Identity.Bearer")]
    private async Task<IResult> SetWidgetOnDashboard(
        [FromRoute] Guid dashboardId,
        [FromBody] SetWidgetOnDashboardRequest request,
        IRequestContext context,
        IMediator mediator)
    {
        var command = request.ToSetWidgetCommand(context.UserId, dashboardId);

        var result = await mediator.Send(command);

        if (result.IsFailure)
            return result.ToResultsDetails();

        return Results.NoContent();
    }
}
