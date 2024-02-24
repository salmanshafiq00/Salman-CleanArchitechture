using CleanArchitechture.Application.Features.Identity.Commands;
using CleanArchitechture.Web.Extensions;

namespace WebApi.Web.Endpoints;

public class Accounts : EndpointGroupBase
{
    private static readonly string RefreshTokenKey = "X-Refresh-Token";
    private static readonly string Authorization = nameof(Authorization);

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
        IHttpContextAccessor context)
    {
        var accessToken = context.HttpContext?.Request.Headers[Authorization].ToString().Replace("Bearer ", "");
        var refreshToken = context.HttpContext?.Request.Cookies[RefreshTokenKey];

        if (string.IsNullOrEmpty(refreshToken) && string.IsNullOrEmpty(accessToken))
        {
            return Results.BadRequest("Token Invalid");
        }

        var result = await sender.Send(new RefreshTokenRequestCommand(accessToken, refreshToken));

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
            context?.HttpContext?.Response.Cookies.Append(RefreshTokenKey, refreshToken, cookieOptions);
        }
    }
}

