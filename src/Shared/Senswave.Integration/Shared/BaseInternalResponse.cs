using Senswave.Abstractions.Resulting;

namespace Senswave.Integration.Shared;

public record BaseInternalResponse
{
    public InternalRequestStatus StatusCode { get; set; } = InternalRequestStatus.Success;
    public Error Error { get; set; } = Error.None;

    public bool IsSuccess => StatusCode == InternalRequestStatus.Success;
    public bool IsFailure => StatusCode == InternalRequestStatus.Failure;

    public static BaseInternalResponse Success() => new() { StatusCode = InternalRequestStatus.Success };
    public static BaseInternalResponse Failure(Error error) => new() { StatusCode = InternalRequestStatus.Failure, Error = error };
    public static BaseInternalResponse Failure() => new() { StatusCode = InternalRequestStatus.Failure };
}