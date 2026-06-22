using Senswave.Devices.Application.Operations.Features.DeleteOperation;

namespace Senswave.Devices.Api.Operations.DeleteOperation;

internal static class DeleteOperationExtensions
{
    internal static DeleteOperationCommand ToDeleteOperationCommand(this Guid operationId, Guid userId) => new()
    {
        OperationId = operationId,
        UserId = userId
    };
}
