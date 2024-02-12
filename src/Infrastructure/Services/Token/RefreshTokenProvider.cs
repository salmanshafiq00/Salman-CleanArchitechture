using System.Security.Cryptography;

namespace CleanArchitechture.Infrastructure.Services.Token;

internal sealed class RefreshTokenProvider
    : IRefreshTokenProvider
{
    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }
}
