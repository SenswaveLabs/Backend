using Senswave.DataSources.Application.Brokers.Clients.ClientState;

namespace Senswave.DataSources.Api.Brokers.Clients.GetClientState;

internal sealed class GetClientStateEndpoint : IEndpoint
{
    public void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("datasources/brokers/{brokerId:guid}/clients", GetStatus)
            .MapToApiVersion(1)
            .WithTags(DataSourcesModule.BrokerClientTag)
            .WithGroupName(DataSourcesModule.GroupName)
            .Produces<GetClientStateResponse>(200)
            .Produces(401)
            .Produces<ErrorProblemDetails>(400)
            .Produces<ErrorProblemDetails>(404);
    }

    [Authorize(AuthenticationSchemes = "Identity.Bearer")]
    private async Task<IResult> GetStatus([FromRoute] Guid brokerId, IMediator mediator, IRequestContext context)
    {
        var query = new ClientStateQuery
        {
            BrokerId = brokerId,
            UserId = context.UserId
        };

        var result = await mediator.Send(query);

        if (result.IsFailure)
            return result.ToResultsDetails();

        var response = result.ToClientStateResponse();

        return Results.Ok(response);
    }
}

