using Senswave.Devices.Domain.Operations.Types.Base;

namespace Senswave.Devices.Domain.Operations.Types.Integer;

public class IntegerOperationConfiguration : BaseOperationConfiguration
{
    [JsonPropertyName("min")]
    public int Min { get; set; } = int.MinValue;

    [JsonPropertyName("max")]
    public int Max { get; set; } = int.MaxValue;
}
