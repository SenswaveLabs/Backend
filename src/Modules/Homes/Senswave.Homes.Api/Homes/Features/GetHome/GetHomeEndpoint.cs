using Senswave.Homes.Application.Homes.Features.GetHome;

namespace Senswave.Homes.Api.Homes.Features.GetHome;

internal sealed class GetHomeEndpoint : IEndpoint
{
    public void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("homes/{homeId:guid}", GetHome)
            .MapToApiVersion(1)
            .WithTags(HomesModule.HomesTag)
            .WithGroupName(HomesModule.GroupName)
            .Produces<GetHomeResponse>(200)
            .Produces(401)
            .Produces<ErrorProblemDetails>(404)
            .Produces<ErrorProblemDetails>(400);
    }

    [Authorize(AuthenticationSchemes = "Identity.Bearer")]
    private async Task<IResult> GetHome([FromRoute] Guid homeId, IMediator mediator, IRequestContext context)
    {
        var query = new GetHomeQuery
        {
            UserId = context.UserId,
            HomeId = homeId
        };

        var result = await mediator.Send(query);

        if (result.IsFailure)
            return result.ToResultsDetails();

        var response = result.ToHomeResponse(context.UserId);
        return Results.Ok(response);
    }
}

