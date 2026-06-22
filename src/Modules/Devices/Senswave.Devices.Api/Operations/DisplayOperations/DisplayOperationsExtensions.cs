using Senswave.Abstractions.Resulting;
using Senswave.Devices.Domain.Operations.Entities;
using Senswave.Devices.Domain.Operations.Extensions;

namespace Senswave.Devices.Api.Operations.DisplayOperations;

internal static class DisplayOperationsExtensions
{
    internal static DisplayOperationsResponse ToDisplayOperationResponse(this Result<IEnumerable<Operation>> result) => new()
    {
        Items = [.. result.Data.Select(o => o.ToDto())]
    };

    internal static OperationDto ToDto(this Operation operation) => new()
    {
        Id = operation.Id,
        Name = operation.Name,
        Type = operation.Type.FromOperationType()
    };
}
