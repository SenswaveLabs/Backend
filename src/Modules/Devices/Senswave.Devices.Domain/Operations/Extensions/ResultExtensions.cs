namespace Senswave.Devices.Domain.Operations.Extensions;

public static class ResultExtensions
{
    public static Result<T> ToResult<T>(this FluentValidation.Results.ValidationResult result)
    {
        var errors = result.Errors
            .Select(x => new Error(x.ErrorCode, ErrorType.Validation, x.ErrorMessage))
            .ToArray();

        return Result<T>.Failure(errors);
    }

    public static Result ToResult(this FluentValidation.Results.ValidationResult result)
    {
        var errors = result.Errors
            .Select(x => new Error(x.ErrorCode, ErrorType.Validation, x.ErrorMessage))
            .ToArray();

        return Result.Failure(errors);
    }
}
