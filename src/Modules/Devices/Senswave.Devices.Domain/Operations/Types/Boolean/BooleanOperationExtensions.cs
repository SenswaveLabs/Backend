using Senswave.Devices.Domain.Operations.Entities;
using System.Text.Json;

namespace Senswave.Devices.Domain.Operations.Types.Boolean;

internal static class BooleanOperationExtensions
{
    internal static BooleanOperation AsBooleanOperation(
        this Operation operation,
        ILogger<IOperation> logger)
    {
        var configuration = JsonSerializer
            .Deserialize<BooleanOperationConfiguration>(operation.Configuration)!;

        return new BooleanOperation(operation, configuration, logger);
    }
}
