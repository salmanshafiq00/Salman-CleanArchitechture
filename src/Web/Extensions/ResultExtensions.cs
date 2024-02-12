using CleanArchitechture.Application.Common.Enums;
using CleanArchitechture.Application.Common.Models;

namespace CleanArchitechture.Web.Extensions;

public static class ResultExtensions
{
    public static T Match<T>(
        this Result result,
        Func<T> onSucceed,
        Func<T> onFailed)
    {
        return result.IsSucceed ? onSucceed() : onFailed();
    }

    public static IResult ToProblemDetails(this Result result)
    {
        if (result.IsSucceed) throw new InvalidOperationException();

        return Results.Problem(
            statusCode: (int)result.ErrorType,
            title: result.ErrorType.GetDisplayName(),
            type: GetType(result.ErrorType),
            extensions: new Dictionary<string, object?>
            {
                {"errors", result.Errors }
            });
    }

    static string GetType(ErrorType errorType) =>
        errorType switch
        {
            ErrorType.Validation => "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            ErrorType.Unauthorized => "https://tools.ietf.org/html/rfc7235#section-3.1",
            ErrorType.Forbidden => "https://tools.ietf.org/html/rfc7231#section-6.5.3",
            ErrorType.NotFound => "https://tools.ietf.org/html/rfc7231#section-6.5.4",
            ErrorType.Conflict => "https://tools.ietf.org/html/rfc7231#section-6.5.8",
            _ => "https://tools.ietf.org/html/rfc7231#section-6.6.1"
        };

}
