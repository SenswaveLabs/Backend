using Senswave.Automations.Domain.Entities;
using System.Text.Json.Nodes;

namespace Senswave.Automations.Application.Models;

public class ResultModel
{
    public Guid Id { get; set; } = Guid.Empty;
    public Guid OperationId { get; set; } = Guid.Empty;

    public string OperationName { get; set; } = string.Empty;

    public JsonValue ValueToSend { get; set; } = JsonValue.Create(string.Empty);


    public static ResultModel ToDto(AutomationResult result, IDictionary<Guid, string> operationIdToOperationName) => new()
    {
        Id = result.Id,
        OperationId = result.OperationId,
        OperationName = operationIdToOperationName[result.OperationId],
        ValueToSend = result.ValueToSend
    };
}