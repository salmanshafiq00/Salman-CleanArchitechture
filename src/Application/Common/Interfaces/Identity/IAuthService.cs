using CleanArchitechture.Application.Common.Models;
using CleanArchitechture.Application.Features.Identity.Models;

namespace CleanArchitechture.Application.Common.Interfaces.Identity;

public interface IAuthService
{
    Task<Result<AuthenticatedResponse>> Login(string username, string password, CancellationToken cancellation = default);
    Task<Result<AuthenticatedResponse>> RefreshToken(string accessToken, string refreshToken, CancellationToken cancellation = default);
    Task<(Result Result, string UserId)> ForgotPassword(string email);
    Task<(Result Result, string UserId)> ResetPassword(string email);
}
