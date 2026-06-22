using Senswave.DataSources.Application.Brokers.Sessions.GetSession;

namespace Senswave.DataSources.Api.Brokers.Sessions.GetSession;

internal sealed class GetSessionEndpoint : IEndpoint
{
    public void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("datasources/brokers/{brokerId:guid}/sessions/{sessionId:guid}", GetSession)
            .MapToApiVersion(1)
            .WithTags(DataSourcesModule.BrokerSessionsTag)
            .WithGroupName(DataSourcesModule.GroupName)
            .Produces<GetSessionResponse>(200)
            .Produces(401)
            .Produces<ErrorProblemDetails>(400)
            .Produces<ErrorProblemDetails>(404);
    }

    [Authorize(AuthenticationSchemes = "Identity.Bearer")]
    private async Task<IResult> GetSession([FromRoute] Guid brokerId, [FromRoute] Guid sessionId, IMediator mediator, IRequestContext context)
    {
        var query = new GetSessionQuery
        {
            UserId = context.UserId,
            BrokerId = brokerId,
            SessionId = sessionId
        };

        var result = await mediator.Send(query);

        if (result.IsFailure)
            return result.ToResultsDetails();

        var response = result.ToResponse();
        return Results.Ok(response);
    }
}
