using Senswave.DataSources.Application.Brokers.Sessions.GetSessions;

namespace Senswave.DataSources.Api.Brokers.Sessions.GetSessions;

internal sealed class GetSessionsEndpoint : IEndpoint
{
    public void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("datasources/brokers/{brokerId:guid}/sessions", GetSessions)
            .MapToApiVersion(1)
            .WithTags(DataSourcesModule.BrokerSessionsTag)
            .WithGroupName(DataSourcesModule.GroupName)
            .Produces<GetSessionsResponse>(200)
            .Produces(401)
            .Produces<ErrorProblemDetails>(400)
            .Produces<ErrorProblemDetails>(404);
    }

    [Authorize(AuthenticationSchemes = "Identity.Bearer")]
    private async Task<IResult> GetSessions(IMediator mediator, IRequestContext context, [FromRoute] Guid brokerId, [FromQuery] int page = 1, [FromQuery] int size = 5)
    {
        var query = new GetSessionsQuery
        {
            UserId = context.UserId,
            BrokerId = brokerId,
            Page = page,
            Size = size
        };

        var result = await mediator.Send(query);

        if (result.IsFailure)
            return result.ToResultsDetails();

        var response = result.ToResponse();
        return Results.Ok(response);
    }
}

