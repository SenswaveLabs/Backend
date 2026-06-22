namespace Senswave.Devices.Domain.Devices.Services;

public interface IDeviceAccessService
{
    Task<Result> IsOwner(Guid userId, Guid deviceId, CancellationToken cancellationToken);
    Task<Result> CanManageHome(Guid userId, Guid homeId, CancellationToken cancellationToken);
    Task<Result> CanDisplayHome(Guid userId, Guid homeId, CancellationToken cancellationToken);
    Task<Result> CanDisplay(Guid userId, Guid deviceId, CancellationToken cancellationToken);
    Task<Result> CanAct(Guid userId, Guid deviceId, CancellationToken cancellationToken);
    Task<Result> CanManage(Guid userId, Guid deviceId, CancellationToken cancellationToken);
}
