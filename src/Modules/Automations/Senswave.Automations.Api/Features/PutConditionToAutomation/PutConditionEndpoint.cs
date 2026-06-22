namespace Senswave.Automations.Api.Features.PutConditionToAutomation;

internal sealed class PutConditionEndpoint : IEndpoint
{
    public void Map(IEndpointRouteBuilder app)
    {
        app.MapPut("automations/conditions/{automationId:guid}", PutConditionToAutomation)
            .MapToApiVersion(1)
            .WithTags(AutomationsModule.AutomationsTag)
            .WithGroupName(AutomationsModule.GroupName)
            .Produces(200)
            .Produces(401)
            .Produces<ErrorProblemDetails>(400)
            .Produces<ErrorProblemDetails>(404);
    }

    [Authorize(AuthenticationSchemes = "Identity.Bearer")]
    private async Task<IResult> PutConditionToAutomation(
        [FromRoute] Guid automationId,
        [FromBody] PutConditionRequest request,
        IMediator mediator,
        IRequestContext context)
    {
        var command = automationId.ToCommand(context.UserId, request.OperationId, request.ConditionType, request.ConditionConfiguration);

        var result = await mediator.Send(command);

        if (result.IsFailure)
            return result.ToResultsDetails();

        return Results.Ok();
    }
}