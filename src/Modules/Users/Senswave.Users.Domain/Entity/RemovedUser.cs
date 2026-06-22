using Senswave.Abstractions.Entities;
using Senswave.Users.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Senswave.Users.Domain.Entity;

public class RemovedUser : AuditableEntity
{
    [MaxLength(AllowedLengths.Descriptions.MaxLength)]
    public string Reason { get; set; } = string.Empty;

    public string SecurityStamp { get; set; } = string.Empty;

    // Email is hashed after successfully sending removal email
    public string HashedEmail { get; set; } = string.Empty;

    public UserRemovalStatus Status { get; set; } = UserRemovalStatus.UserDataRemoved;
}
