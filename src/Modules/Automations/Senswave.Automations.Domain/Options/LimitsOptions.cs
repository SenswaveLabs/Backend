namespace Senswave.Automations.Domain.Options;

public class LimitsOptions
{
    public const string SectionName = $"{AutomationOptions.SectionName}:Limits";

    public int AutomationsPerHome { get; set; } = 16;
}
