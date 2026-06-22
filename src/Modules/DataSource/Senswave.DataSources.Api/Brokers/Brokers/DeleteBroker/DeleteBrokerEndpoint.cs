namespace Senswave.DataSources.Api.Brokers.Brokers.DeleteBroker;

internal sealed class DeleteBrokerEndpoint : IEndpoint
{
    public void Map(IEndpointRouteBuilder app)
    {
        app.MapDelete("datasources/brokers/{brokerId:guid}", DeleteBroker)
            .MapToApiVersion(1)
            .WithTags(DataSourcesModule.BrokersTag)
            .WithGroupName(DataSourcesModule.GroupName)
            .Produces(204)
            .Produces<ErrorProblemDetails>(404);
    }

    [Authorize(AuthenticationSchemes = "Identity.Bearer")]
    private async Task<IResult> DeleteBroker([FromRoute] Guid brokerId, IMediator mediator, IRequestContext context)
    {
        var command = brokerId.ToDeleteBrokerCommand(context.UserId);

        var result = await mediator.Send(command);

        if (result.IsFailure)
            return result.ToResultsDetails();

        return Results.NoContent();
    }
}
