using Senswave.Homes.Application.Sharings.Features.GetSharings;

namespace Senswave.Homes.Api.Sharings.GetSharings;

internal sealed class GetSharingsEndpoint : IEndpoint
{
    public void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("homes/sharings", GetHomeSharings)
            .MapToApiVersion(1)
            .WithTags(HomesModule.SharingTag)
            .WithGroupName(HomesModule.GroupName)
            .Produces<GetShraingsResponse>(200)
            .Produces(401)
            .Produces<ErrorProblemDetails>(404)
            .Produces<ErrorProblemDetails>(400);
    }

    [Authorize(AuthenticationSchemes = "Identity.Bearer")]
    private async Task<IResult> GetHomeSharings([FromQuery] Guid homeId, IMediator mediator, IRequestContext context)
    {
        var query = new GetSharingsQuery
        {
            HomeId = homeId,
            UserId = context.UserId
        };

        var results = await mediator.Send(query);

        if (results.IsFailure)
            return results.ToResultsDetails();

        var response = results.ToResponse();
        return Results.Ok(response);
    }
}

