using Senswave.Users.Domain.Interfaces;

namespace Senswave.Users.Api.Legal.GetPrivacyPolicy;

internal sealed class GetPrivacyPolicyEndpoint : IEndpoint, IPublicEndpoint
{
    public void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("legal/privacy", GetPrivacyPolicy)
            .MapToApiVersion(1)
            .WithTags(UsersModule.LegalTag)
            .WithGroupName(UsersModule.GroupName)
            .Produces<GetPrivacyPolicyResponse>(200)
            .Produces<ErrorProblemDetails>(500);
    }

    private async Task<IResult> GetPrivacyPolicy(ILegalService legalService)
    {
        var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));

        var privacyPolicy = await legalService.GetPrivacyPolicy(cts.Token);

        if (privacyPolicy.IsFailure)
            return privacyPolicy.ToResultsDetails();

        var response = new GetPrivacyPolicyResponse
        {
            Id = privacyPolicy.Data.Id,
            Version = privacyPolicy.Data.Version,
            Summary = privacyPolicy.Data.Summary,
            Content = privacyPolicy.Data.Content,
            CreatedAtUtc = privacyPolicy.Data.CreatedAtUtc,
        };

        return Results.Ok(response);
    }
}
