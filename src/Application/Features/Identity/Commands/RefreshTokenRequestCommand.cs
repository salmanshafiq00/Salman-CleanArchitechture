using CleanArchitechture.Application.Common.Abstractions.Identity;
using CleanArchitechture.Application.Features.Identity.Models;

namespace CleanArchitechture.Application.Features.Identity.Commands;

public record RefreshTokenRequestCommand(string AccessToken, string RefreshToken)
    : ICommand<AuthenticatedResponse>;


internal sealed class RefreshTokenCommandHandler(IAuthService authService)
    : ICommandHandler<RefreshTokenRequestCommand, AuthenticatedResponse>
{
    public async Task<Result<AuthenticatedResponse>> Handle(RefreshTokenRequestCommand request, CancellationToken cancellationToken)
    {
        return await authService
            .RefreshToken(request.AccessToken, request.RefreshToken, cancellationToken);
    }
}
