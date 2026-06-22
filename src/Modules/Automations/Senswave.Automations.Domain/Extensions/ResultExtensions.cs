namespace Senswave.Automations.Domain.Extensions;

internal static class ResultExtensions
{
    public static Result ToResult(this FluentValidation.Results.ValidationResult result)
    {
        var errors = result.Errors
            .Select(x => new Error(x.ErrorCode, ErrorType.Validation, x.ErrorMessage))
            .ToArray();

        return Result.Failure(errors);
    }
}