using Senswave.Automations.Domain.Entities;

namespace Senswave.Automations.Application.Models;

public class AutomationModel
{
    public Guid Id { get; set; } = Guid.Empty;

    public Guid HomeId { get; set; } = Guid.Empty;

    public string Name { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;

    public string ConditionConnector { get; set; } = string.Empty;
    public bool IsEnabled { get; set; } = false;

    public IList<ConditionModel> Conditions { get; set; } = [];

    public IList<ResultModel> Results { get; set; } = [];

    public static AutomationModel ToDto(Automation automation, IDictionary<Guid, string> operationIdToOperationName) => new()
    {
        Id = automation.Id,
        HomeId = automation.HomesReference.HomeId,
        Name = automation.Name,
        Icon = automation.Icon,

        ConditionConnector = automation.ConditionsConnector.ToString(),
        IsEnabled = automation.IsEnabled,

        Conditions = automation.Conditions
                .Select(x => ConditionModel.ToDto(x, operationIdToOperationName))
                .ToList(),
        Results = automation.Results
                .Select(x => ResultModel.ToDto(x, operationIdToOperationName))
                .ToList()
    };
}