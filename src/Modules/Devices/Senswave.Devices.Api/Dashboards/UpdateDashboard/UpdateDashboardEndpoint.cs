namespace Senswave.Devices.Api.Dashboards.UpdateDashboard;

internal sealed class UpdateDashboardEndpoint : IEndpoint
{
    public void Map(IEndpointRouteBuilder app)
    {
        app.MapPatch("devices/dashboards/{dashboardId:guid}", PatchDashboard)
            .MapToApiVersion(1)
            .WithTags(DevicesModule.DashboardsTag)
            .WithGroupName(DevicesModule.GroupName)
            .Produces(204)
            .ProducesProblem(401)
            .Produces<ErrorProblemDetails>(404)
            .Produces<ErrorProblemDetails>(409)
            .Produces<ErrorProblemDetails>(400);
    }

    [Authorize(AuthenticationSchemes = "Identity.Bearer")]
    private async Task<IResult> PatchDashboard([FromRoute] Guid dashboardId, [FromBody] UpdateDashboardRequest request, IMediator mediator, IRequestContext context)
    {
        var command = request.ToCommand(context.UserId, dashboardId);
        var response = await mediator.Send(command);

        if (response.IsFailure)
            return response.ToResultsDetails();

        return Results.NoContent();
    }
}

