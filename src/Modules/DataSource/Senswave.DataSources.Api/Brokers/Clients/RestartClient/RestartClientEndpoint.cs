namespace Senswave.DataSources.Api.Brokers.Clients.RestartClient;

internal sealed class RestartClientEndpoint : IEndpoint
{
    public void Map(IEndpointRouteBuilder app)
    {
        app.MapPatch("datasources/brokers/{brokerId:guid}/clients/restart", RestartClient)
            .MapToApiVersion(1)
            .WithTags(DataSourcesModule.BrokerClientTag)
            .WithGroupName(DataSourcesModule.GroupName)
            .Produces(204)
            .Produces(401)
            .Produces<ErrorProblemDetails>(400)
            .Produces<ErrorProblemDetails>(404);
    }

    [Authorize(AuthenticationSchemes = "Identity.Bearer")]
    private async Task<IResult> RestartClient([FromRoute] Guid brokerId, IMediator mediator, IRequestContext context)
    {
        var command = brokerId.ToRestartClientCommand(context.UserId);

        var result = await mediator.Send(command);

        if (result.IsFailure)
            return result.ToResultsDetails();

        return Results.NoContent();
    }
}

