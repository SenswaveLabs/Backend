using Senswave.Abstractions.Entities;
namespace Senswave.Users.Domain.Entity;

public class EmailNotification : AuditableEntity
{
    public string Email { get; set; } = string.Empty;

    public string NotificationType { get; set; } = string.Empty;
}
