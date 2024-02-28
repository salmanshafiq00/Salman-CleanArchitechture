using Microsoft.AspNetCore.Identity;

namespace CleanArchitechture.Infrastructure.Identity;

public static class IdentityResultExtensions
{
    //public static Result ToApplicationResult(this IdentityResult result)
    //{
    //    return result.Succeeded
    //        ? Result.Success()
    //        : Result.Failure(result.Errors.FirstOrDefault().(x => new Error(x.Code, x.Description, ErrorType.Failure)));
    //}
    //public static Result<TValue> ToApplicationResult<TValue>(this IdentityResult result, dynamic value)
    //{
    //    return result.Succeeded
    //        ? Result<TValue>.Success(value)
    //        : Result.Failure<TValue>(result.Errors.Select(e => e.Description).ToArray());
    //}
}
