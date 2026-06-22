namespace Senswave.DataSources.Api.Brokers.Brokers.CreateSubscribtion;

internal sealed class CreateSubscribtionEndpoint : IEndpoint
{
    public void Map(IEndpointRouteBuilder app)
    {
        app.MapPost("datasources/brokers/{brokerId:guid}/subscriptions", Create)
            .MapToApiVersion(1)
            .WithTags(DataSourcesModule.BrokerSubscribtionsTag)
            .WithGroupName(DataSourcesModule.GroupName)
            .Produces<CreateSubscribtionResponse>(201)
            .Produces(401)
            .Produces<ErrorProblemDetails>(400)
            .Produces<ErrorProblemDetails>(404)
            .Produces<ErrorProblemDetails>(409);
    }

    [Authorize(AuthenticationSchemes = "Identity.Bearer")]
    private async Task<IResult> Create(
        [FromRoute] Guid brokerId,
        CreateSubscribtionRequest request,
        IMediator mediator,
        IRequestContext context)
    {
        var command = request.ToCommand(brokerId, context.UserId);
        var result = await mediator.Send(command);

        if (result.IsFailure)
            return result.ToResultsDetails();

        var response = result.ToResponse();
        return Results.Created($"/{result.Data}", response);
    }
}
