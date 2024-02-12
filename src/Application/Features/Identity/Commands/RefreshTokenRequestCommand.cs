using CleanArchitechture.Application.Common.Interfaces.Identity;
using CleanArchitechture.Application.Common.Models;
using CleanArchitechture.Application.Features.Identity.Models;

namespace CleanArchitechture.Application.Features.Identity.Commands;

public record RefreshTokenRequestCommand(string AccessToken)
    : IRequest<Result<AuthenticatedResponse>>
{
    public string RefreshToken { get; set; } = string.Empty;
}

internal sealed class RefreshTokenCommandHandler(IAuthService authService)
    : IRequestHandler<RefreshTokenRequestCommand, Result<AuthenticatedResponse>>
{
    public async Task<Result<AuthenticatedResponse>> Handle(RefreshTokenRequestCommand request, CancellationToken cancellationToken)
    {
        return await authService
            .RefreshToken(request.AccessToken, request.RefreshToken, cancellationToken);
    }
}
