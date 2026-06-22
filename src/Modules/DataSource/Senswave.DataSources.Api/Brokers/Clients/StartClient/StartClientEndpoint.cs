namespace Senswave.DataSources.Api.Brokers.Clients.StartClient;

internal sealed class StartClientEndpoint : IEndpoint
{
    public void Map(IEndpointRouteBuilder app)
    {
        app.MapPost("datasources/brokers/{brokerId:guid}/clients", StartClient)
            .MapToApiVersion(1)
            .WithTags(DataSourcesModule.BrokerClientTag)
            .WithGroupName(DataSourcesModule.GroupName)
            .Produces(204)
            .Produces(401)
            .Produces<ErrorProblemDetails>(400)
            .Produces<ErrorProblemDetails>(404);
    }

    [Authorize(AuthenticationSchemes = "Identity.Bearer")]
    private async Task<IResult> StartClient([FromRoute] Guid brokerId, StartClientDto request, IMediator mediator, IRequestContext context)
    {
        var command = request.ToCommand(brokerId, context.UserId);

        var result = await mediator.Send(command);

        if (result.IsFailure)
            return result.ToResultsDetails();

        return Results.NoContent();
    }
}

