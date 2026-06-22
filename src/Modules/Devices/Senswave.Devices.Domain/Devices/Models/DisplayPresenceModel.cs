using Senswave.Devices.Domain.Devices.Enums;

namespace Senswave.Devices.Domain.Devices.Models;

public record DisplayPresenceModel
{
    public DevicePresenceType Type { get; init; }

    public bool? Value { get; init; }

    public DateTime? LastSeenAtUtc { get; init; }
}