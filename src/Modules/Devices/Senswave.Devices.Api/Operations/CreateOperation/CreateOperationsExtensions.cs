using Senswave.Abstractions.Resulting;
using Senswave.Devices.Application.Operations.Features.CreateOperation;
using Senswave.Devices.Domain.Operations.Entities;
using Senswave.Devices.Domain.Operations.Extensions;

namespace Senswave.Devices.Api.Operations.CreateOperation;

internal static class CreateOperationsExtensions
{
    internal static OperationCreatedResponse ToOperationCreatedResponse(this Result<Operation> result) => new()
    {
        Id = result.Data.Id
    };

    public static CreateOperationCommand ToCommand(this CreateOperationRequest request, Guid userId) => new()
    {
        DeviceId = request.DeviceId,
        Name = request.Name,
        Configuration = request.Configuration,
        UserId = userId,
        Topic = request.Topic,
        Type = request.Type.ToOperationType()
    };
}