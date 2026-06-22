namespace Senswave.DataSources.Api.Brokers.Brokers.DeleteSubscribtion;

internal sealed class DeleteSubscribtionEndpoint : IEndpoint
{
    public void Map(IEndpointRouteBuilder app)
    {
        app.MapDelete("datasources/brokers/{brokerId:guid}/subscriptions/{subscriptionId:guid}", Delete)
            .MapToApiVersion(1)
            .WithTags(DataSourcesModule.BrokerSubscribtionsTag)
            .WithGroupName(DataSourcesModule.GroupName)
            .Produces(204)
            .Produces(401)
            .Produces<ErrorProblemDetails>(404)
            .Produces<ErrorProblemDetails>(409);
    }

    [Authorize(AuthenticationSchemes = "Identity.Bearer")]
    private async Task<IResult> Delete(
        [FromRoute] Guid brokerId,
        [FromRoute] Guid subscriptionId,
        IMediator mediator,
        IRequestContext context)
    {
        var command = subscriptionId.ToDeleteSubscribtionCommand(brokerId, context.UserId);
        var result = await mediator.Send(command);

        if (result.IsFailure)
            return result.ToResultsDetails();

        return Results.NoContent();
    }
}
