namespace Senswave.Automations.Application.Features.DeleteCondition;

public class DeleteConditionCommand : ICommand
{
    public Guid UserId { get; set; } = Guid.Empty;
    public Guid ConditionId { get; set; } = Guid.Empty;
}