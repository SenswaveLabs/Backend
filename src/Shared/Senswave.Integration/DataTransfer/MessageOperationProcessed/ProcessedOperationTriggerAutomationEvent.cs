namespace Senswave.Integration.DataTransfer.MessageOperationProcessed;

public record ProcessedOperationTriggerAutomationEvent
{
    public Guid HomeId { get; set; }
    public Guid OperationId { get; set; }
}