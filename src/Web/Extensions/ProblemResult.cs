using CleanArchitechture.Web.Helpers;

namespace CleanArchitechture.Web.Extensions;

public class ProblemResult<T>(Result<T> result) : IResult
{
    public async Task ExecuteAsync(HttpContext httpContext)
    {
        var response = httpContext.Response;

        response.ContentType = HttpContentTypeConstants.ApplicationJson;
        //response.StatusCode = result.

        await response.WriteAsJsonAsync(result, cancellationToken: httpContext.RequestAborted).ConfigureAwait(false);

    }
}
