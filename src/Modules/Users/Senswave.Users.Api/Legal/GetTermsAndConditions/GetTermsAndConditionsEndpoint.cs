using Senswave.Users.Domain.Interfaces;

namespace Senswave.Users.Api.Legal.GetTermsAndConditions;

public class GetTermsAndConditionsEndpoint : IEndpoint, IPublicEndpoint
{
    public void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("legal/terms", GetTerms)
            .MapToApiVersion(1)
            .WithTags(UsersModule.LegalTag)
            .WithGroupName(UsersModule.GroupName)
            .Produces<GetTermsAndConditionsResponse>(200)
            .Produces<ErrorProblemDetails>(500);
    }

    private async Task<IResult> GetTerms(ILegalService legalService)
    {
        var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));

        var terms = await legalService.GetTermsAndConditions(cts.Token);

        if (terms.IsFailure)
            return terms.ToResultsDetails();

        var response = new GetTermsAndConditionsResponse
        {
            Id = terms.Data.Id,
            Version = terms.Data.Version,
            Summary = terms.Data.Summary,
            Content = terms.Data.Content,
            CreatedAtUtc = terms.Data.CreatedAtUtc,
        };

        return Results.Ok(response);
    }
}
