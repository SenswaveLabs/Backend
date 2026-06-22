namespace Senswave.Automations.Api.Features.DeleteAutomation;

internal sealed class DeleteAutomationEndpoint : IEndpoint
{
    public void Map(IEndpointRouteBuilder app)
    {
        app.MapDelete("automations/{automationId:guid}", DeleteAutomation)
            .MapToApiVersion(1)
            .WithTags(AutomationsModule.AutomationsTag)
            .WithGroupName(AutomationsModule.GroupName)
            .Produces(204)
            .Produces(401)
            .Produces<ErrorProblemDetails>(400)
            .Produces<ErrorProblemDetails>(404);
    }

    [Authorize(AuthenticationSchemes = "Identity.Bearer")]
    private async Task<IResult> DeleteAutomation([FromRoute] Guid automationId, IMediator mediator, IRequestContext context)
    {
        var command = automationId.ToDeleteCommand(context.UserId);
        var result = await mediator.Send(command);

        if (result.IsFailure)
            return result.ToResultsDetails();

        return Results.NoContent();
    }
}

