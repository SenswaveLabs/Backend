namespace Senswave.Automations.Application.Features.DeleteResult;

public class DeleteResultCommand : ICommand
{
    public Guid ResultId { get; set; }

    public Guid UserId { get; set; }
}