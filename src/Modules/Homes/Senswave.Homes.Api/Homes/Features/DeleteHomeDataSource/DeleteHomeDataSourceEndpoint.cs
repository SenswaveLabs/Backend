namespace Senswave.Homes.Api.Homes.Features.DeleteHomeDataSource;

internal sealed class DeleteHomeDataSourceEndpoint : IEndpoint
{
    public void Map(IEndpointRouteBuilder app)
    {
        app.MapDelete("homes/{homeId:guid}/datasource", DeleteHomeDataSource)
            .MapToApiVersion(1)
            .WithTags(HomesModule.HomesTag)
            .WithGroupName(HomesModule.GroupName)
            .Produces(204)
            .Produces(401)
            .Produces<ErrorProblemDetails>(400)
            .Produces<ErrorProblemDetails>(404);
    }

    [Authorize(AuthenticationSchemes = "Identity.Bearer")]
    private async Task<IResult> DeleteHomeDataSource([FromRoute] Guid homeId, IMediator mediator, IRequestContext context)
    {
        var command = homeId.ToDeleteHomeDataSourceCommand(context.UserId);
        var result = await mediator.Send(command);

        if (result.IsFailure)
            return result.ToResultsDetails();

        return Results.NoContent();
    }
}
