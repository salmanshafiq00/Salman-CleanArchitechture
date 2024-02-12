namespace CleanArchitechture.Infrastructure.Services.Token;

internal interface IRefreshTokenProvider
{
    string GenerateRefreshToken();
}
