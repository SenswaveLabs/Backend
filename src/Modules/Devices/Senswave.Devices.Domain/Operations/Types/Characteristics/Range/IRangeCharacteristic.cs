namespace Senswave.Devices.Domain.Operations.Types.Characteristics.Range;

public interface IRangeCharacteristic<T> : IRangeCharacteristic;

public interface IRangeCharacteristic
{
    Task<Result> ValidateStep(object step);

    Task<Result<JsonObject>> GetDisplayRange();
}
