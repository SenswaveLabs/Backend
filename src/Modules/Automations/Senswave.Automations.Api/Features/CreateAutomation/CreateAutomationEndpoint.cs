namespace Senswave.Automations.Api.Features.CreateAutomation;

internal sealed class CreateAutomationEndpoint : IEndpoint
{
    public void Map(IEndpointRouteBuilder app)
    {
        app.MapPost("automations", CreateAutomation)
            .MapToApiVersion(1)
            .WithTags(AutomationsModule.AutomationsTag)
            .WithGroupName(AutomationsModule.GroupName)
            .Produces<AutomationCreatedResponse>(201)
            .Produces(401)
            .Produces<ErrorProblemDetails>(404)
            .Produces<ErrorProblemDetails>(400);
    }

    [Authorize(AuthenticationSchemes = "Identity.Bearer")]
    private async Task<IResult> CreateAutomation([FromBody] CreateAutomationRequest request, IMediator mediator, IRequestContext context)
    {
        var command = request.ToCommand(context.UserId);
        var result = await mediator.Send(command);

        if (result.IsFailure)
            return result.ToResultsDetails();

        var response = result.ToAutomationCreatedResponse();
        return Results.Created($"/{result.Data.Id}", response);
    }
}

