namespace Senswave.Automations.Api.Features.UpdateAutomation;

internal sealed class UpdateAutomationEndpoint : IEndpoint
{
    public void Map(IEndpointRouteBuilder app)
    {
        app.MapPatch("automations/{automationId:guid}", UpdateAutomation)
            .MapToApiVersion(1)
            .WithTags(AutomationsModule.AutomationsTag)
            .WithGroupName(AutomationsModule.GroupName)
            .Produces(204)
            .Produces(401)
            .Produces<ErrorProblemDetails>(400)
            .Produces<ErrorProblemDetails>(404)
            .Produces<ErrorProblemDetails>(409);
    }

    [Authorize(AuthenticationSchemes = "Identity.Bearer")]
    private async Task<IResult> UpdateAutomation([FromRoute] Guid automationId, [FromBody] UpdateAutomationRequest request, IMediator mediator, IRequestContext context)
    {
        var command = request.ToCommand(context.UserId, automationId);
        var result = await mediator.Send(command);

        if (result.IsFailure)
            return result.ToResultsDetails();

        return Results.NoContent();
    }
}

