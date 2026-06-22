using Senswave.DataSources.Application.Brokers.Brokers.Features.GetSubscribtions;

namespace Senswave.DataSources.Api.Brokers.Brokers.GetSubscribtions;

internal sealed class GetSubscribtionsEndpoint : IEndpoint
{
    public void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("datasources/brokers/{brokerId:guid}/subscriptions", GetSubscribtions)
            .MapToApiVersion(1)
            .WithTags(DataSourcesModule.BrokerSubscribtionsTag)
            .WithGroupName(DataSourcesModule.GroupName)
            .Produces<GetSubscribtionsResponse>(200)
            .Produces(401)
            .Produces<ErrorProblemDetails>(400)
            .Produces<ErrorProblemDetails>(404);
    }

    [Authorize(AuthenticationSchemes = "Identity.Bearer")]
    private async Task<IResult> GetSubscribtions(
        [FromRoute] Guid brokerId,
        IMediator mediator,
        IRequestContext context,
        [FromQuery] int page = 1,
        [FromQuery] int size = 10)
    {
        var query = new GetSubscribtionsQuery
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
