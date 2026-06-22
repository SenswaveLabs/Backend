using Senswave.Devices.Domain.Operations.Entities;
using System.Text.Json;

namespace Senswave.Devices.Domain.Operations.Types.Option;

internal static class OptionOperationExtensions
{
    internal static OptionOperation AsOptionsOperation(
        this Operation operation,
        ILogger<IOperation> logger)
    {
        var configuration = JsonSerializer
            .Deserialize<OptionOperationConfiguration>(operation.Configuration)!;

        return new OptionOperation(operation, configuration, logger);
    }
}
