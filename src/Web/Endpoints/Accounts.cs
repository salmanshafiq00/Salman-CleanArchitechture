using CleanArchitechture.Application.Features.Identity.Commands;
using CleanArchitechture.Web.Extensions;

namespace WebApi.Web.Endpoints;

public class Accounts : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .MapPost(Login)
            .MapPost(RefreshToken, "RefreshToken");
    }

    public async Task<IResult> Login(
        ISender sender, 
        IHttpContextAccessor context, 
        LoginRequestCommand command)
    {
        var result = await sender.Send(command);

        SetRefreshTokenInCookie(context, result?.Value?.RefreshToken, result?.Value?.RefreshTokenExpiresOn);

        return result.Match(
            onSucceed: () => Results.Ok(result),
            onFailed: result.ToProblemDetails);
    }
    public async Task<IResult> RefreshToken(
        ISender sender, 
        IHttpContextAccessor context, 
        RefreshTokenRequestCommand command)
    {
        command.RefreshToken = context.HttpContext.Request.Cookies["refreshToken"];

        if (string.IsNullOrEmpty(command.RefreshToken))
        {
            return Results.NotFound("Refresh Token Invalidated");
        }

        var result = await sender.Send(command);

        SetRefreshTokenInCookie(context, result?.Value?.RefreshToken, result?.Value?.RefreshTokenExpiresOn);

        return result.Match(
            onSucceed: () => Results.Ok(result),
            onFailed: result.ToProblemDetails);
    }
    private static void SetRefreshTokenInCookie(
        IHttpContextAccessor context, 
        string refreshToken, 
        DateTime? expiresOn)
    {
        if (expiresOn is not null)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = expiresOn,
            };
            context?.HttpContext?.Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
        }
    }
}

