namespace Senswave.Users.Api.Legal.GetTermsAndConditions;

public class GetTermsAndConditionsResponse
{
    public Guid Id { get; set; }

    public string Version { get; set; } = string.Empty;

    public string Content { get; set; } = string.Empty;

    public string Summary { get; set; } = string.Empty;

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}
