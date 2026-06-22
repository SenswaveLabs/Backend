namespace Senswave.Devices.Domain.Devices.Options;

public class DevicesOptions
{
    public const string SectionName = "Modules:Devices";

    public LimitsOptions Limits { get; set; } = new();
}
