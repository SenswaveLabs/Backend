namespace Senswave.Automations.Api.Features.CreateAutomation;

public class CreateAutomationRequest
{
    public Guid HomeId { get; set; } = Guid.Empty;

    public string Name { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;

    public string ConditionConnector { get; set; } = string.Empty;

    public IList<ConditionDto> Conditions { get; set; } = [];

    public IList<ResultDto> Results { get; set; } = [];
}