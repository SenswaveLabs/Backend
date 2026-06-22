using Senswave.Integration.Shared;

namespace Senswave.Integration.Devices.OperationNameById;

public record OperationNameByIdsResponse : BaseInternalResponse
{
    public Dictionary<Guid, string> IdToName { get; set; } = [];
}