using Senswave.Devices.Application.Dashboards.Features.DisplayDashboard;

namespace Senswave.Devices.Api.Dashboards.DisplayDashboard;

internal class DisplayDashboardEndpoint : IEndpoint
{
    public void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("devices/dashboards/{dashboardId}/display", DisplayDashboard)
            .MapToApiVersion(1)
            .WithTags(DevicesModule.DashboardsTag)
            .WithGroupName(DevicesModule.GroupName)
            .Produces<DisplayDashboardResponse>(200)
            .ProducesProblem(401)
            .Produces<ErrorProblemDetails>(404)
            .Produces<ErrorProblemDetails>(400);
    }

    [Authorize(AuthenticationSchemes = "Identity.Bearer")]
    private async Task<IResult> DisplayDashboard([FromRoute] Guid dashboardId, IMediator mediator, IRequestContext context)
    {
        var query = new DisplayDashboardQuery
        {
            UserId = context.UserId,
            DashboardId = dashboardId,
        };

        var result = await mediator.Send(query);

        if (result.IsFailure)
            return result.ToResultsDetails();

        var response = result.ToDisplayDashboardResponse();
        return Results.Ok(response);
    }
}
