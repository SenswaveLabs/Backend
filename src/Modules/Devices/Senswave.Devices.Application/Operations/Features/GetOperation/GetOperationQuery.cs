namespace Senswave.Devices.Application.Operations.Features.GetOperation;

public class GetOperationQuery : IQuery<ExtendedOperationModel>
{
    public Guid UserId { get; set; }

    public Guid OperationId { get; set; }
}