namespace Senswave.DataSources.Api.Brokers.Clients.StopClient;

internal sealed class StopClientEndpoint : IEndpoint
{
    public void Map(IEndpointRouteBuilder app)
    {
        app.MapDelete("datasources/brokers/{brokerId:guid}/clients", StopConnection)
            .MapToApiVersion(1)
            .WithTags(DataSourcesModule.BrokerClientTag)
            .WithGroupName(DataSourcesModule.GroupName)
            .Produces(204)
            .Produces(401)
            .Produces<ErrorProblemDetails>(400)
            .Produces<ErrorProblemDetails>(404);
    }

    [Authorize(AuthenticationSchemes = "Identity.Bearer")]
    private async Task<IResult> StopConnection([FromRoute] Guid brokerId, IMediator mediator, IRequestContext context)
    {
        var command = brokerId.ToStopClientCommand(context.UserId);

        var result = await mediator.Send(command);

        if (result.IsFailure)
            return result.ToResultsDetails();

        return Results.NoContent();
    }
}

