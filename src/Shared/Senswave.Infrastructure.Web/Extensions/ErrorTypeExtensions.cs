using Microsoft.AspNetCore.Http;
using Senswave.Abstractions.Resulting;
using System.ComponentModel;

namespace Senswave.Infrastructure.Web.Extensions;

public static class ErrorTypeExtensions
{
    public static int GetStatusCode(this ErrorType errorType) => errorType switch
    {
        ErrorType.Validation => StatusCodes.Status400BadRequest,
        ErrorType.NotFound => StatusCodes.Status404NotFound,
        ErrorType.Conflict => StatusCodes.Status409Conflict,
        ErrorType.Failure => StatusCodes.Status400BadRequest,
        ErrorType.ServerFail => StatusCodes.Status500InternalServerError,
        _ => throw new InvalidEnumArgumentException()
    };

    public static string GetTitle(this ErrorType errorType) => errorType switch
    {
        ErrorType.Validation => "Bad Request",
        ErrorType.NotFound => "Not Found",
        ErrorType.Conflict => "Conflict",
        ErrorType.Failure => "Bad Request",
        ErrorType.ServerFail => "Server Error",
        _ => throw new InvalidEnumArgumentException()
    };
}
