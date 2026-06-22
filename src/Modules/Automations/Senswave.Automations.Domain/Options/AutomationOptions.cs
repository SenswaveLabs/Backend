
namespace Senswave.Automations.Domain.Options;

public class AutomationOptions
{
    public const string SectionName = "Modules:Automations";

    public LimitsOptions Limits { get; set; } = new();
}
