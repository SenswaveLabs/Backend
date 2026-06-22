using Senswave.Devices.Domain.Operations.Entities;
using System.Text.Json;

namespace Senswave.Devices.Domain.Operations.Types.Integer;

internal static class IntegerOperationExtenstions
{
    internal static IntegerOperation AsIntegerOperation(
        this Operation operation,
        ILogger<IOperation> logger)
    {
        var configuration = JsonSerializer
            .Deserialize<IntegerOperationConfiguration>(operation.Configuration)!;

        return new IntegerOperation(operation, configuration, logger);
    }
}
