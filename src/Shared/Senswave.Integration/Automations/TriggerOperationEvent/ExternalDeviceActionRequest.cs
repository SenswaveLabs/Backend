namespace Senswave.Integration.Automations.TriggerOperationEvent;

public record ExternalDeviceActionRequest
{
    public List<TriggerOperationWithValue> OperationsWithValues { get; set; } = [];
}