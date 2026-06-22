namespace Senswave.DataSources.Api.Brokers.Brokers.UpdateBroker;

internal sealed class UpdateBrokerEndpoint : IEndpoint
{
    public void Map(IEndpointRouteBuilder app)
    {
        app.MapPatch("datasources/brokers/{brokerId:guid}", UpdateBroker)
            .MapToApiVersion(1)
            .WithTags(DataSourcesModule.BrokersTag)
            .WithGroupName(DataSourcesModule.GroupName)
            .Produces(204)
            .Produces(401)
            .Produces<ErrorProblemDetails>(400)
            .Produces<ErrorProblemDetails>(404)
            .Produces<ErrorProblemDetails>(409);
    }

    [Authorize(AuthenticationSchemes = "Identity.Bearer")]
    private async Task<IResult> UpdateBroker([FromRoute] Guid brokerId, UpdateBrokerRequest request, IMediator mediator, IRequestContext context)
    {
        var command = request.ToCommand(context.UserId, brokerId);

        var result = await mediator.Send(command);

        if (result.IsFailure)
            return result.ToResultsDetails();

        return Results.NoContent();
    }
}
