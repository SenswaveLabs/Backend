namespace Senswave.Automations.Api.Features.PutResultToAutomation;

public class PutResultRequest
{
    public Guid OperationId { get; set; } = Guid.NewGuid();
    public String Value { get; set; } = String.Empty;
}