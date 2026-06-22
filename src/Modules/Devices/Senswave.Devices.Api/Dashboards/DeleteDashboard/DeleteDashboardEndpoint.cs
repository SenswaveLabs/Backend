using Senswave.Devices.Application.Dashboards.Features.DeleteDashboard;

namespace Senswave.Devices.Api.Dashboards.DeleteDashboard;

internal sealed class DeleteDashboardEndpoint : IEndpoint
{
    public void Map(IEndpointRouteBuilder app)
    {
        app.MapDelete("devices/dashboards/{dashboardId:guid}", DeleteDashboard)
            .MapToApiVersion(1)
            .WithTags(DevicesModule.DashboardsTag)
            .WithGroupName(DevicesModule.GroupName)
            .Produces(204)
            .ProducesProblem(401)
            .Produces<ErrorProblemDetails>(400)
            .Produces<ErrorProblemDetails>(404);
    }

    [Authorize(AuthenticationSchemes = "Identity.Bearer")]
    private async Task<IResult> DeleteDashboard([FromRoute] Guid dashboardId, IMediator mediator, IRequestContext context)
    {
        var command = new DeleteDashboardCommand
        {
            UserId = context.UserId,
            DashboardId = dashboardId
        };

        var response = await mediator.Send(command);

        if (response.IsFailure)
            return response.ToResultsDetails();

        return Results.NoContent();
    }
}

