using Microsoft.IdentityModel.Tokens;

namespace CleanArchitechture.Infrastructure.Services.Token;

internal interface IAccessTokenValidator
{
    Task<TokenValidationResult> ValidateTokenAsync(string accessToken);
}
