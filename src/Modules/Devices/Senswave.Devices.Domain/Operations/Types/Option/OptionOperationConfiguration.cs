using Senswave.Devices.Domain.Operations.Types.Base;
using Senswave.Devices.Domain.Operations.Types.Option.Models;

namespace Senswave.Devices.Domain.Operations.Types.Option;

public class OptionOperationConfiguration : BaseOperationConfiguration
{
    [JsonPropertyName("options")]
    public List<OptionInfo> Options { get; set; } = [];
}
