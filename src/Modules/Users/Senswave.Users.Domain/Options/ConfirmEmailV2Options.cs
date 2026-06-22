namespace Senswave.Users.Domain.Options;

public class ConfirmEmailV2Options
{
    public const string SectionName = $"{UsersOptions.SectionName}:ConfirmEmailV2";

    public string SuccessRedirect { get; set; } = string.Empty;

    public string FailureRedirect { get; set; } = string.Empty;
}
