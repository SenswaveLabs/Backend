namespace Senswave.Automations.Api.Features.PutResultToAutomation;

internal sealed class PutResultEndpoint : IEndpoint
{

    public void Map(IEndpointRouteBuilder app)
    {
        app.MapPut("automations/results/{automationId:guid}", PutResultToAutomation)
            .MapToApiVersion(1)
            .WithTags(AutomationsModule.AutomationsTag)
            .WithGroupName(AutomationsModule.GroupName)
            .Produces(200)
            .Produces(401)
            .Produces<ErrorProblemDetails>(400)
            .Produces<ErrorProblemDetails>(404);
    }


    [Authorize(AuthenticationSchemes = "Identity.Bearer")]
    private async Task<IResult> PutResultToAutomation(
        [FromRoute] Guid automationId,
        [FromBody] PutResultRequest request,
        IMediator mediator,
        IRequestContext context)
    {
        var command = automationId.ToCommand(context.UserId, request.OperationId, request.Value);

        var result = await mediator.Send(command);

        if (result.IsFailure)
            return result.ToResultsDetails();

        return Results.Ok();
    }

}