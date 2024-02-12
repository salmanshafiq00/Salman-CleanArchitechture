namespace CleanArchitechture.Infrastructure.Services.Token;

internal interface IAccessTokenProvider
{
    Task<(string AccessToken, int ExpiresInMinutes)> GenerateAccessTokenAsync(string userId);
}
