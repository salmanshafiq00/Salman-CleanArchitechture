using CleanArchitechture.Application.Features.Identity.Commands;
using CleanArchitechture.Application.Features.Identity.Models;
using CleanArchitechture.Web.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Web.Endpoints;

public class Accounts : EndpointGroupBase
{
    private static readonly string RefreshTokenKey = "X-Refresh-Token";
    private static readonly string Authorization = nameof(Authorization);

    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .MapPost(Login, "Login", "Login")
            .MapPost(RefreshToken, "RefreshToken")
            .MapPost(Logout, "Logout");
    }

    [ProducesResponseType(typeof(AuthenticatedResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IResult> Login(
        ISender sender,
        IHttpContextAccessor context,
        LoginRequestCommand command)
    {
        Result<AuthenticatedResponse> result = await sender.Send(command);

        if (result.IsSuccess)
        {
            SetRefreshTokenInCookie(context, result.Value.RefreshToken, result.Value.RefreshTokenExpiresOn);
        }

        return result.Match(
             onSuccess: () => TypedResults.Ok(result.Value),
             onFailure: result.ToProblemDetails);
    }

    [ProducesResponseType(typeof(AuthenticatedResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IResult> RefreshToken(
        ISender sender,
        IHttpContextAccessor context)
    {
        if (!context.HttpContext.Request.Headers.TryGetValue(Authorization, out var authorizationHeader))
        {
            return TypedResults.BadRequest("Invalid Token");
        }
        if (!context.HttpContext.Request.Cookies.TryGetValue(RefreshTokenKey, out var refreshToken))
        {
            return TypedResults.BadRequest("Invalid Token");
        }
        var accessToken = authorizationHeader.ToString().Replace("Bearer ", "");

        var result = await sender.Send(new RefreshTokenRequestCommand(accessToken, refreshToken));

        if (!result.IsSuccess) return result.ToProblemDetails();

        SetRefreshTokenInCookie(context, result.Value.RefreshToken, result.Value.RefreshTokenExpiresOn);

        return result.Match(
             onSuccess: () => TypedResults.Ok(result.Value),
             onFailure: result.ToProblemDetails);
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IResult> Logout(
        ISender sender,
        IHttpContextAccessor context)
    {
        if (!context.HttpContext.Request.Headers.TryGetValue(Authorization, out var authorizationHeader))
        {
            return TypedResults.BadRequest("Invalid Token");
        }
        var accessToken = authorizationHeader.ToString().Replace("Bearer ", "");

        var result = await sender.Send(new LogoutRequestCommand(accessToken));

        SetRefreshTokenInCookie(context, string.Empty, DateTime.Now);

        return result.Match(
             onSuccess: () => TypedResults.Ok(),
             onFailure: result.ToProblemDetails);
    }

    private static void SetRefreshTokenInCookie(
        IHttpContextAccessor context,
        string refreshToken,
        DateTime expiresOn)
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Expires = expiresOn,
            Secure = true,
            SameSite = SameSiteMode.None,
        };
        context.HttpContext.Response.Cookies.Append(RefreshTokenKey, refreshToken, cookieOptions);
    }
}

