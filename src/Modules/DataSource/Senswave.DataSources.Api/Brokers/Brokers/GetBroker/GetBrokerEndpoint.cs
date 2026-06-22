using Senswave.DataSources.Application.Brokers.Brokers.Features.GetBroker;

namespace Senswave.DataSources.Api.Brokers.Brokers.GetBroker;

internal sealed class GetBrokerEndpoint : IEndpoint
{
    public void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("datasources/brokers/{brokerId:guid}", GetBroker)
            .MapToApiVersion(1)
            .WithTags(DataSourcesModule.BrokersTag)
            .WithGroupName(DataSourcesModule.GroupName)
            .Produces<GetBrokerResponse>(200)
            .Produces(401)
            .Produces<ErrorProblemDetails>(404);
    }

    [Authorize(AuthenticationSchemes = "Identity.Bearer")]
    private async Task<IResult> GetBroker([FromRoute] Guid brokerId, IMediator mediator, IRequestContext context)
    {
        var query = new GetBrokerQuery
        {
            UserId = context.UserId,
            BrokerId = brokerId
        };

        var result = await mediator.Send(query);

        if (result.IsFailure)
            return result.ToResultsDetails();

        var response = result.ToBrokerResponse();
        return Results.Ok(response);
    }
}
