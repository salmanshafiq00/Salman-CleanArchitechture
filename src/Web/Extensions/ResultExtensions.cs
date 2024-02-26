using CleanArchitechture.Application.Common.Enums;
using CleanArchitechture.Application.Common.Models;
using Microsoft.AspNetCore.Http.HttpResults;

namespace CleanArchitechture.Web.Extensions;

public static class ResultExtensions
{
    public static TResult Match<TResult>(
        this Result result,
        Func<TResult> onSucceed,
        Func<TResult> onFailed)
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


    public static JsonHttpResult<Result> ToEmptyProblemDetails(this Result result)
    {
        if (result.IsSucceed) throw new InvalidOperationException();

        return TypedResults.Json(result, statusCode: (int)result.ErrorType);
    }

    public static JsonHttpResult<Result<T>> ToProblemDetails<T>(this Result<T> result)
    {
        if (result.IsSucceed) throw new InvalidOperationException();

        return TypedResults.Json(result, statusCode: (int)result.ErrorType);
    }

    public static JsonHttpResult<Result<T>> ToProblemDetails<T>(
        this Result<T> result,
        string message,
        ErrorType errorType)
    {
        if (result.IsSucceed) throw new InvalidOperationException();

        return TypedResults.Json(new Result<T>(result.Value, false, errorType, message), statusCode: (int)errorType);
    }

    public static JsonHttpResult<Result<T>> ToCustomProblemDetails<T>(
        string message,
        ErrorType errorType)
    {

        return TypedResults.Json(new Result<T>(default, false, errorType, message), statusCode: (int)errorType);
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
