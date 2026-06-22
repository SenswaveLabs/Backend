using Microsoft.AspNetCore.Http;
using Senswave.Abstractions.Resulting;

namespace Senswave.Infrastructure.Web.Extensions;

public static class ResultExtensions
{
    public static IResult ToResultsDetails(this Result result)
    {
        if (result.IsSuccess)
            throw new InvalidOperationException("Operation is success but was treated as error.");

        if (result.Errors.Length == 0)
            throw new ArgumentException("Operation is failure but found no error.");

        if (result.Errors.Any(x => x as Error == Error.None))
            throw new ArgumentException("Operation is failure but found none error.");

        var error = result.Errors.First();

        return Results.Problem(
            statusCode: error.Type.GetStatusCode(),
            title: error.Type.GetTitle(),
            extensions: new Dictionary<string, object?>
            {
                {"errors", result.Errors }
            });
    }
}
