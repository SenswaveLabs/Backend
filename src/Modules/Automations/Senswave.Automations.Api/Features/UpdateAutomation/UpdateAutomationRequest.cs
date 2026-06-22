using Senswave.Automations.Api.Features.CreateAutomation;

namespace Senswave.Automations.Api.Features.UpdateAutomation;

public class UpdateAutomationRequest
{
    public string Name { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;

    public string ConditionConnector { get; set; } = string.Empty;

    public IList<ConditionDto> Conditions { get; set; } = [];
    public IList<ResultDto> Results { get; set; } = [];
}