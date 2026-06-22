namespace Senswave.Devices.Domain.Operations.Types.Base;

public class BaseOperationConfiguration : IOperationConfiguration
{
    [JsonPropertyName("isJson")]
    public bool IsJson { get; set; }

    [JsonPropertyName("jsonNames")]
    public string[] JsonNames { get; set; } = [];

    [JsonPropertyName("saveOnUserAction")]
    public bool SaveOnUserAction { get; set; } = true;


    [JsonIgnore]
    public bool IsNotJson => !IsJson;
}
