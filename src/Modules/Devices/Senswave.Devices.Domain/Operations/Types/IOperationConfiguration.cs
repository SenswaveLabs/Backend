namespace Senswave.Devices.Domain.Operations.Types;

public interface IOperationConfiguration
{
    bool IsJson { get; }

    string[] JsonNames { get; }
}
