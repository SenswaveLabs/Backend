namespace Senswave.Automations.Domain.Types.Condition;

public abstract class BaseCondition
{
    public Guid OperationId { get; set; } = Guid.Empty;

    public abstract bool CheckCondition(JsonValue operationPayload);

    public abstract Task<Result> Validate(CancellationToken cancellationToken);
}