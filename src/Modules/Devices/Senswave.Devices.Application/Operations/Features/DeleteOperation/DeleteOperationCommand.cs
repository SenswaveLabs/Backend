namespace Senswave.Devices.Application.Operations.Features.DeleteOperation;

public class DeleteOperationCommand : ICommand
{
    public Guid UserId { get; set; } = Guid.Empty;

    public Guid OperationId { get; set; } = Guid.Empty;
}