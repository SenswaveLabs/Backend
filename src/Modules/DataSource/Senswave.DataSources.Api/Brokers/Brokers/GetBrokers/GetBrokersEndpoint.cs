using Senswave.DataSources.Application.Brokers.Brokers.Features.GetBrokers;

namespace Senswave.DataSources.Api.Brokers.Brokers.GetBrokers;

internal sealed class GetBrokersEndpoint : IEndpoint
{
    public void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("datasources/brokers", GetBrokers)
            .MapToApiVersion(1)
            .WithTags(DataSourcesModule.BrokersTag)
            .WithGroupName(DataSourcesModule.GroupName)
            .Produces<GetBrokersResponse>(200)
            .Produces(401)
            .Produces<ErrorProblemDetails>(400)
            .Produces<ErrorProblemDetails>(404);
    }

    [Authorize(AuthenticationSchemes = "Identity.Bearer")]
    private async Task<IResult> GetBrokers(IMediator mediator, IRequestContext context, [FromQuery] int page = 1, [FromQuery] int size = 10)
    {
        var query = new GetBrokersQuery
        {
            UserId = context.UserId,
            Page = page,
            Size = size
        };

        var result = await mediator.Send(query);

        if (result.IsFailure)
            return result.ToResultsDetails();

        var response = result.ToBrokersResponse();
        return Results.Ok(response);
    }
}
