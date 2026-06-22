using Senswave.Devices.Domain.Operations.Entities;
using System.Text.Json;

namespace Senswave.Devices.Domain.Operations.Types.Number;

internal static class NumberOperationExtensions
{
    internal static NumberOperation AsNumberOperation(
        this Operation operation,
        ILogger<IOperation> logger)
    {
        var configuration = JsonSerializer
            .Deserialize<NumberOperationConfiguration>(operation.Configuration)!;

        return new NumberOperation(operation, configuration, logger);
    }
}
