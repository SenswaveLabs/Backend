using Senswave.Abstractions.Resulting;
using Senswave.Devices.Application.Operations.Features.GetOperation;
using Senswave.Devices.Domain.Operations.Extensions;

namespace Senswave.Devices.Api.Operations.GetOperation;

internal static class GetOperationExtensions
{
    internal static GetOperationResponse ToOperationResponse(this Result<ExtendedOperationModel> result) => new()
    {
        Id = result.Data.Operation.Id,
        Name = result.Data.Operation.Name,
        Configuration = result.Data.Operation.Configuration,
        Type = result.Data.Operation.Type.FromOperationType(),
        Topic = result.Data.Topic
    };
}
