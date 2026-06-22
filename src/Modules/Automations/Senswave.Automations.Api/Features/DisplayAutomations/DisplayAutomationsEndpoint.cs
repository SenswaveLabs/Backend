using Senswave.Automations.Application.Features.DisplayAutomations;

namespace Senswave.Automations.Api.Features.DisplayAutomations;

internal sealed class DisplayAutomationsEndpoint : IEndpoint
{
    public void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("automations/display", DisplayAutomations)
            .MapToApiVersion(1)
            .WithTags(AutomationsModule.AutomationsTag)
            .WithGroupName(AutomationsModule.GroupName)
            .Produces<DisplayAutomationsResponse>(200)
            .Produces(401)
            .Produces<ErrorProblemDetails>(404)
            .Produces<ErrorProblemDetails>(400);
    }

    [Authorize(AuthenticationSchemes = "Identity.Bearer")]
    private async Task<IResult> DisplayAutomations([FromQuery] Guid homeId, IMediator mediator, IRequestContext context)
    {
        var query = new DisplayAutomationsQuery
        {
            HomeId = homeId,
            UserId = context.UserId
        };

        var result = await mediator.Send(query);

        if (result.IsFailure)
            return result.ToResultsDetails();

        var response = result.Data.ToResponse();
        return Results.Ok(response);
    }
}

