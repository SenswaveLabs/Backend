using Senswave.Devices.Domain.Operations.Types.Base;

namespace Senswave.Devices.Domain.Operations.Types.Boolean;

public class BooleanOperationConfiguration : BaseOperationConfiguration
{
    [JsonPropertyName("trueValue")]
    public JsonValue? TrueValue { get; set; }

    [JsonPropertyName("falseValue")]
    public JsonValue? FalseValue { get; set; }

    [JsonIgnore]
    public JsonValue EffectiveTrueValue => TrueValue ?? JsonValue.Create(true)!;

    [JsonIgnore]
    public JsonValue EffectiveFalseValue => FalseValue ?? JsonValue.Create(false)!;
}
