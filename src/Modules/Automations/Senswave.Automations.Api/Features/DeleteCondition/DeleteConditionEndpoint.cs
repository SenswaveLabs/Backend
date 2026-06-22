
namespace Senswave.Automations.Api.Features.DeleteCondition;

internal sealed class DeleteConditionEndpoint : IEndpoint
{

    public void Map(IEndpointRouteBuilder app)
    {
        app.MapDelete("automations/conditions/{conditionId:guid}", DeleteAutomationCondition)
            .MapToApiVersion(1)
            .WithTags(AutomationsModule.AutomationsTag)
            .WithGroupName(AutomationsModule.GroupName)
            .Produces(204)
            .Produces(401)
            .Produces<ErrorProblemDetails>(400)
            .Produces<ErrorProblemDetails>(404);
    }

    [Authorize(AuthenticationSchemes = "Identity.Bearer")]
    private async Task<IResult> DeleteAutomationCondition([FromRoute] Guid conditionId, IMediator mediator, IRequestContext context)
    {
        var command = conditionId.ToDeleteConditionCommand(context.UserId);
        var result = await mediator.Send(command);

        if (result.IsFailure)
            return result.ToResultsDetails();

        return Results.NoContent();
    }
}