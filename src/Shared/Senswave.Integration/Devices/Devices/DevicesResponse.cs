using Senswave.Integration.Shared;

namespace Senswave.Integration.Devices.Devices;

public record DevicesResponse : BaseInternalResponse
{
    public List<Guid> Devices { get; set; } = [];
}
