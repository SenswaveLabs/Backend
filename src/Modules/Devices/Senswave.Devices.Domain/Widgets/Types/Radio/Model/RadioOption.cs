namespace Senswave.Devices.Domain.Widgets.Types.Radio.Model;

public class RadioOption
{
    [JsonPropertyName("optionName")]
    public string OptionName { get; set; } = string.Empty;

    [JsonPropertyName("displayName")]
    public string DisplayName { get; set; } = string.Empty;

    [JsonPropertyName("icon")]
    public string Icon { get; set; } = string.Empty;
}
