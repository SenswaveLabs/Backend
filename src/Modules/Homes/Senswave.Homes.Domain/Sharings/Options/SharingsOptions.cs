namespace Senswave.Homes.Domain.Sharings.Options;

public class SharingsOptions
{
    public const string SectionName = $"{HomeModuleOptions.SectionName}:Sharings";

    // Changing this requires adjusting UI as well
    public int PasswordLength { get; set; } = 8;

    public int ActiveInvitationsPerOwner { get; set; } = 10;

    public int InvitationExpiresInSeconds { get; set; } = 60 * 15; // 15 minutes
}
