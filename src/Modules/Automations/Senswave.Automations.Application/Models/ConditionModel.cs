using Senswave.Automations.Domain.Entities;
using System.Text.Json.Nodes;

namespace Senswave.Automations.Application.Models;

public class ConditionModel
{
    public Guid Id { get; set; } = Guid.Empty;
    public Guid OperationId { get; set; } = Guid.Empty;

    public string OperationName { get; set; } = string.Empty;

    public string ConditionType { get; set; } = string.Empty;

    public JsonObject ConditionConfiguration { get; set; } = [];


    public static ConditionModel ToDto(AutomationCondition condition, IDictionary<Guid, string> operationIdToOperationName) => new()
    {
        Id = condition.Id,
        OperationId = condition.OperationId,
        OperationName = operationIdToOperationName[condition.OperationId],
        ConditionType = condition.ConditionType.ToString(),
        ConditionConfiguration = condition.ConditionConfiguration
    };
}