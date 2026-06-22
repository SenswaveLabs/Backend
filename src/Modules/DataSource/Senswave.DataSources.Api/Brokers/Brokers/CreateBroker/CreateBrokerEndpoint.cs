namespace Senswave.DataSources.Api.Brokers.Brokers.CreateBroker;

internal sealed class CreateBrokerEndpoint : IEndpoint
{
    public void Map(IEndpointRouteBuilder app)
    {
        app.MapPost($"datasources/brokers", Create)
            .MapToApiVersion(1)
            .WithTags(DataSourcesModule.BrokersTag)
            .WithGroupName(DataSourcesModule.GroupName)
            .Produces<BrokerCreatedResponse>(201)
            .Produces(401)
            .Produces<ErrorProblemDetails>(400)
            .Produces<ErrorProblemDetails>(409);
    }

    [Authorize(AuthenticationSchemes = "Identity.Bearer")]
    private async Task<IResult> Create(CreateBrokerRequest request, IMediator mediator, IRequestContext context)
    {
        var command = request.ToCommand(context.UserId);
        var result = await mediator.Send(command);

        if (result.IsFailure)
            return result.ToResultsDetails();

        var response = result.ToBrokerCreatedResponse();
        return Results.Created($"/{result.Data.Id}", response);
    }
}
