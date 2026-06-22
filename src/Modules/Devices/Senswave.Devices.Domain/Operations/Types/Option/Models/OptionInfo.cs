namespace Senswave.Devices.Domain.Operations.Types.Option.Models;

public class OptionInfo
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("value")]
    public JsonValue Value { get; set; } = JsonValue.Create("");
}
