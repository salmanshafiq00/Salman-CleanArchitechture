using System.Net.Sockets;
using System.Net;
using Application.Constants;
using CleanArchitechture.Application.Common.Models;
using CleanArchitechture.Application.Features.Identity.Models;
using CleanArchitechture.Infrastructure.Services.Token;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using CleanArchitechture.Infrastructure.OptionsSetup.Jwt;
using CleanArchitechture.Application.Common.Abstractions.Identity;
using CleanArchitechture.Application.Common.Abstractions;

namespace CleanArchitechture.Infrastructure.Identity;

internal sealed class AuthService(
    UserManager<ApplicationUser> userManager,
    ITokenProviderService tokenProvider,
    IApplicationDbContext dbContext,
    IOptionsSnapshot<JwtOptions> jwtOptions)
    : IAuthService
{
    private readonly JwtOptions _jwtOptions = jwtOptions.Value;

    public async Task<Result<AuthenticatedResponse>> Login(
        string username,
        string password,
        CancellationToken cancellation = default)
    {
        var user = await userManager.FindByEmailAsync(username)
           ?? await userManager.FindByNameAsync(username);

        //Guard.Against.NotFound(nameof(username), user, ErrorMessages.WRONG_USERNAME_PASSWORD);

        if (user is null)
        {
            return Result.Failure<AuthenticatedResponse>(Error.NotFound(nameof(user) ,ErrorMessages.WRONG_USERNAME_PASSWORD));
        }

        var result = await userManager.CheckPasswordAsync(user, password);

        if (!result)
        {
            return Result.Failure<AuthenticatedResponse>(Error.NotFound(nameof(user), ErrorMessages.WRONG_USERNAME_PASSWORD));
        }

        var (accessToken, expiresInMinutes) = await tokenProvider.GenerateAccessTokenAsync(user.Id);

        var authResponse = new AuthenticatedResponse
        {
            AccessToken = accessToken,
            ExpiresInMinutes = expiresInMinutes,
        };

        var activeRefreshToken = await dbContext.RefreshTokens
            .FirstOrDefaultAsync(x => x.ApplicationUserId == user.Id, cancellation);

        // if refresh token is already active return it with authResponse
        // otherwise generate new one
        if (activeRefreshToken is { IsActive: true })
        {
            authResponse.RefreshToken = activeRefreshToken.Token;
            authResponse.RefreshTokenExpiresOn = activeRefreshToken.Expires;

            return Result.Success(authResponse);
        }

        // Generate new refresh token
        var refreshToken = new RefreshToken
        {
            Token = tokenProvider.GenerateRefreshToken(),
            Expires = DateTime.UtcNow.AddDays(_jwtOptions.RefreshTokenExpires),
            Created = DateTime.UtcNow,
            CreatedByIp = GetIpAddress(),
            ApplicationUserId = user.Id
        };

        authResponse.RefreshToken = refreshToken.Token;
        authResponse.RefreshTokenExpiresOn = refreshToken.Expires;

        // New refresh token added to database.
        dbContext.RefreshTokens.Add(refreshToken);
        await dbContext.SaveChangesAsync(cancellation);

        return !string.IsNullOrEmpty(accessToken)
            ? Result.Success(authResponse)
            : Result.Failure<AuthenticatedResponse>(Error.NotFound(nameof(user), ErrorMessages.WRONG_USERNAME_PASSWORD));
    }

    public async Task<Result<AuthenticatedResponse>> RefreshToken(
        string accessToken,
        string refreshToken,
        CancellationToken cancellation = default)
    {
        // Get RefreshToken entity from db using input refresh token
        var existedRefreshToken = await dbContext.RefreshTokens
            .FirstOrDefaultAsync(x => x.Token == refreshToken, cancellation);

        // existedRefreshToken is null means the invalid token
        if (existedRefreshToken is null)
        {
            return Result.Failure<AuthenticatedResponse>(Error.Validation("Token", ErrorMessages.TOKEN_DID_NOT_MATCH));
        }

        // Check user token is active
        if (!existedRefreshToken.IsActive)
        {
            return Result.Failure<AuthenticatedResponse>(Error.Validation("Token", ErrorMessages.TOKEN_NOT_ACTIVE));
        }

        // Revoke Current Refresh Token
        existedRefreshToken.Revoked = DateTime.Now;

        // Get ClaimPrincipal from accessToken
        var claimsPrincipalResult = GetClaimsPrincipalFromToken(accessToken);

        if (claimsPrincipalResult.IsFailure)
        {
            return Result.Failure<AuthenticatedResponse>(claimsPrincipalResult.Error);
        }

        // Get Identity UserId  from ClaimPrincipal
        var userId = (claimsPrincipalResult.Value?.FindFirstValue(ClaimTypes.NameIdentifier))
            ?? throw new SecurityTokenException(ErrorMessages.INVALID_TOKEN);

        // Generate new Access Token
        var (token, expiresInMinutes) = await tokenProvider.GenerateAccessTokenAsync(userId);

        // Generate new Refresh Token and create RefreshToken instance to insert into db
        var newRefreshToken = new RefreshToken
        {
            Token = tokenProvider.GenerateRefreshToken(),
            Expires = DateTime.UtcNow.AddDays(_jwtOptions.RefreshTokenExpires),
            Created = DateTime.UtcNow,
            CreatedByIp = GetIpAddress(),
            ApplicationUserId = userId
        };

        var tokenResponse = new AuthenticatedResponse
        {
            AccessToken = token,
            ExpiresInMinutes = expiresInMinutes,
            RefreshToken = newRefreshToken.Token,
            RefreshTokenExpiresOn = newRefreshToken.Expires
        };

        // Save new RefreshToken into DB
        dbContext.RefreshTokens.Add(newRefreshToken);
        await dbContext.SaveChangesAsync(cancellation);

        return !string.IsNullOrEmpty(token)
                    ? Result.Success(tokenResponse)
                    : Result.Failure<AuthenticatedResponse>(Error.NotFound("Token", ErrorMessages.INVALID_TOKEN));

    }

    public Task<(Result Result, string UserId)> ForgotPassword(string email)
    {
        throw new NotImplementedException();
    }

    public Task<(Result Result, string UserId)> ResetPassword(string email)
    {
        throw new NotImplementedException();
    }


    private static string GetIpAddress()
    {
        var host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                return ip.ToString();
            }
        }
        return string.Empty;
    }

    private Result<ClaimsPrincipal> GetClaimsPrincipalFromToken(string accessToken)
    {

        try
        {
            TokenValidationParameters tokenValidationParameters = new()
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = false,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _jwtOptions.Issuer,
                ValidAudience = _jwtOptions.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.SecretKey)),
                ClockSkew = TimeSpan.Zero
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var principal = tokenHandler.ValidateToken(accessToken, tokenValidationParameters, out SecurityToken securityToken);
            if (securityToken is not JwtSecurityToken jwtSecurityToken
                || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                return Result.Failure<ClaimsPrincipal>(Error.Validation("Token", ErrorMessages.INVALID_TOKEN));
            }

            return principal;
        }
        catch
        {
            return Result.Failure<ClaimsPrincipal>(Error.Validation("Token", ErrorMessages.INVALID_TOKEN));
        }
    }
}
