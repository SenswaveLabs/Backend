using Senswave.Devices.Application.Dashboards.Features.GetDashboard;

namespace Senswave.Devices.Api.Dashboards.GetDashboard;

internal sealed class GetDashboardEndpoint : IEndpoint
{
    public void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("devices/dashboards/{dashboardId:guid}", GetDashboard)
            .MapToApiVersion(1)
            .WithTags(DevicesModule.DashboardsTag)
            .WithGroupName(DevicesModule.GroupName)
            .Produces<GetDashboardResponse>(200)
            .ProducesProblem(401)
            .Produces<ErrorProblemDetails>(404)
            .Produces<ErrorProblemDetails>(400);
    }

    [Authorize(AuthenticationSchemes = "Identity.Bearer")]
    private async Task<IResult> GetDashboard([FromRoute] Guid dashboardId, IMediator mediator, IRequestContext context)
    {
        var query = new GetDashboardQuery
        {
            UserId = context.UserId,
            DashboardId = dashboardId,
        };

        var result = await mediator.Send(query);

        if (result.IsFailure)
            return result.ToResultsDetails();

        var data = result.ToResponse();
        return Results.Ok(data);
    }
}

