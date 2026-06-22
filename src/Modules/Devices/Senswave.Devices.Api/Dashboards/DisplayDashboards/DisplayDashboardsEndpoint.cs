using Senswave.Devices.Application.Dashboards.Features.DisplayDashboards;

namespace Senswave.Devices.Api.Dashboards.DisplayDashboards;

internal sealed class DisplayDashboardsEndpoint : IEndpoint
{
    public void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("devices/dashboards/display", DisplayDashboards)
            .MapToApiVersion(1)
            .WithTags(DevicesModule.DashboardsTag)
            .WithGroupName(DevicesModule.GroupName)
            .Produces<DisplayDashboardsResponse>(200)
            .ProducesProblem(401)
            .Produces<ErrorProblemDetails>(404)
            .Produces<ErrorProblemDetails>(400);
    }

    [Authorize(AuthenticationSchemes = "Identity.Bearer")]
    private async Task<IResult> DisplayDashboards([FromQuery] Guid deviceId, IMediator mediator, IRequestContext context)
    {
        var query = new DisplayDashboardsQuery
        {
            UserId = context.UserId,
            DeviceId = deviceId,
        };

        var result = await mediator.Send(query);

        if (result.IsFailure)
            return result.ToResultsDetails();

        var response = result.ToResponse();
        return Results.Ok(response);
    }
}

