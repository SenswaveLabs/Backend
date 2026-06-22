using Senswave.Automations.Application.Features.GetAutomation;
using Senswave.Automations.Application.Models;

namespace Senswave.Automations.Api.Features.GetAutomation;

internal sealed class GetAutomationEndpoint : IEndpoint
{
    public void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("automations/{automationId:guid}", GetAutomation)
            .MapToApiVersion(1)
            .WithTags(AutomationsModule.AutomationsTag)
            .WithGroupName(AutomationsModule.GroupName)
            .Produces<AutomationModel>(200)
            .Produces(401)
            .Produces<ErrorProblemDetails>(404)
            .Produces<ErrorProblemDetails>(400);
    }

    [Authorize(AuthenticationSchemes = "Identity.Bearer")]
    private async Task<IResult> GetAutomation([FromRoute] Guid automationId, IMediator mediator, IRequestContext context)
    {
        var query = new GetAutomationQuery
        {
            AutomationId = automationId,
            UserId = context.UserId
        };

        var result = await mediator.Send(query);

        if (result.IsFailure)
            return result.ToResultsDetails();

        return Results.Ok(result.Data);
    }
}
