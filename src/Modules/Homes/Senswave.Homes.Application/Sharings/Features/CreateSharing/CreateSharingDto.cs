namespace Senswave.Homes.Application.Sharings.Features.CreateSharing;

public class CreateSharingDto
{
    public Guid Id { get; set; }

    public string Password { get; set; } = string.Empty;

    public DateTime ExpiresAtUtc { get; set; }

    public DateTime CreatedAtUtc { get; set; }
}
