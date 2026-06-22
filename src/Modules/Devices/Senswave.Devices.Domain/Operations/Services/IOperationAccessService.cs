namespace Senswave.Devices.Domain.Operations.Services;

public interface IOperationAccessService
{
    Task<Result> CanDisplay(Guid userId, Guid operationId, CancellationToken cancellationToken);
    Task<Result> CanDisplayDevice(Guid userId, Guid deviceId, CancellationToken cancellationToken);
    Task<Result> CanManage(Guid userId, Guid operationId, CancellationToken cancellationToken);
    Task<Result> CanManageDevice(Guid userId, Guid deviceId, CancellationToken cancellationToken);
}
