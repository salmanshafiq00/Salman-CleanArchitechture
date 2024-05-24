namespace CleanArchitechture.Infrastructure.Identity.Services;

internal interface IRefreshTokenProvider
{
    string GenerateRefreshToken();
}
