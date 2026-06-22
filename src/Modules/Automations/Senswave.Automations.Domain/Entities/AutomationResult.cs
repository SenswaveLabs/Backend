namespace Senswave.Automations.Domain.Entities;

public class AutomationResult : AuditableEntity
{
    public Automation Automation { get; set; }
    public Guid OperationId { get; set; }

    [MaxLength(2048)]
    public JsonValue ValueToSend { get; set; } = JsonValue.Create(string.Empty)!;
}
