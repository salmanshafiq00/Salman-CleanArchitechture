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
            .MapPost(Login)
            .MapPost(RefreshToken, "RefreshToken");
    }

    [ProducesResponseType(typeof(AuthenticatedResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(AuthenticatedResponse), StatusCodes.Status404NotFound)]
    public async Task<IResult> Login(
        ISender sender, 
        IHttpContextAccessor context, 
        LoginRequestCommand command)
    {
        Result<AuthenticatedResponse>? result = await sender.Send(command);

        SetRefreshTokenInCookie(context, result?.Value?.RefreshToken, result?.Value?.RefreshTokenExpiresOn);

        return result.IsFailure
            ? result.ToProblemDetails() 
            : TypedResults.Ok(result);
    }

    public async Task<IResult> RefreshToken(
        ISender sender, 
        IHttpContextAccessor context)
    {
        var accessToken = context.HttpContext?.Request.Headers[Authorization].ToString().Replace("Bearer ", "");
        var refreshToken = context.HttpContext?.Request.Cookies[RefreshTokenKey];

        //if (string.IsNullOrEmpty(refreshToken) && string.IsNullOrEmpty(accessToken))
        //{
        //    return ResultExtensions.ToCustomProblemDetails<AuthenticatedResponse>("Invalid Token", ErrorType.NotFound);
        //}

        var result = await sender.Send(new RefreshTokenRequestCommand(accessToken, refreshToken));

        SetRefreshTokenInCookie(context, result?.Value?.RefreshToken, result?.Value?.RefreshTokenExpiresOn);

        return result.IsFailure
            ? result.ToProblemDetails()
            : TypedResults.Ok(result);
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
                Expires = expiresOn
            };
            context?.HttpContext?.Response.Cookies.Append(RefreshTokenKey, refreshToken, cookieOptions);
        }
    }
}

