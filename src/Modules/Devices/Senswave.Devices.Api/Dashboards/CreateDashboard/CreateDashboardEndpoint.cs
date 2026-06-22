namespace Senswave.Devices.Api.Dashboards.CreateDashboard;

internal sealed class CreateDashboardEndpoint : IEndpoint
{
    public void Map(IEndpointRouteBuilder app)
    {
        app.MapPost("devices/dashboards", PostDashboard)
            .MapToApiVersion(1)
            .WithTags(DevicesModule.DashboardsTag)
            .WithGroupName(DevicesModule.GroupName)
            .Produces<CreateDashboardRequest>(201)
            .ProducesProblem(401)
            .Produces<ErrorProblemDetails>(404)
            .Produces<ErrorProblemDetails>(400);
    }

    [Authorize(AuthenticationSchemes = "Identity.Bearer")]
    private async Task<IResult> PostDashboard([FromBody] CreateDashboardRequest request, IMediator mediator, IRequestContext context)
    {
        var command = request.ToCommand(context.UserId);
        var result = await mediator.Send(command);

        if (result.IsFailure)
            return result.ToResultsDetails();

        var response = result.ToCreatedResponse();
        return Results.Created($"/{result.Data.Id}", response);
    }
}

