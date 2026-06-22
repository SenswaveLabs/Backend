namespace Senswave.Homes.Api.Homes.Features.SetHomeDataSource;

internal sealed class SetHomeDataSourceEndpoint : IEndpoint
{
    public void Map(IEndpointRouteBuilder app)
    {
        app.MapPut("homes/{homeId:guid}/datasource", AssignHomeDataSource)
            .MapToApiVersion(1)
            .WithTags(HomesModule.HomesTag)
            .WithGroupName(HomesModule.GroupName)
            .Produces(204)
            .Produces(401)
            .Produces<ErrorProblemDetails>(400)
            .Produces<ErrorProblemDetails>(404);
    }

    [Authorize(AuthenticationSchemes = "Identity.Bearer")]
    private async Task<IResult> AssignHomeDataSource([FromRoute] Guid homeId, [FromBody] AssignHomeDataSourceRequest request, IMediator mediator, IRequestContext context)
    {
        var command = request.ToCommand(context.UserId, homeId);
        var result = await mediator.Send(command);

        if (result.IsFailure)
            return result.ToResultsDetails();

        return Results.NoContent();
    }
}

