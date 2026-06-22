using Senswave.Devices.Domain.Operations.Types.Base;

namespace Senswave.Devices.Domain.Operations.Types.Number;

public class NumberOperationConfiguration : BaseOperationConfiguration
{
    [JsonPropertyName("decimalSeparator")]
    public string DecimalSeparator { get; set; } = ".";

    [JsonPropertyName("min")]
    public double Min { get; set; } = double.MinValue;

    [JsonPropertyName("max")]
    public double Max { get; set; } = double.MaxValue;
}
