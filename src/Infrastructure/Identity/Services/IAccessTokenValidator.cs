using Microsoft.IdentityModel.Tokens;

namespace CleanArchitechture.Infrastructure.Identity.Services;

internal interface IAccessTokenValidator
{
    Task<TokenValidationResult> ValidateTokenAsync(string accessToken);
}
