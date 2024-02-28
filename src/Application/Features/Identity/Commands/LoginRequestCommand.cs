using CleanArchitechture.Application.Common.Interfaces.Identity;
using CleanArchitechture.Application.Features.Identity.Models;

namespace CleanArchitechture.Application.Features.Identity.Commands;

public sealed record LoginRequestCommand(string UserName, string Password)
    : IRequest<Result<AuthenticatedResponse>>;

internal sealed class LoginRequestCommandHandler(IAuthService authService)
    : IRequestHandler<LoginRequestCommand, Result<AuthenticatedResponse>>
{
    public async Task<Result<AuthenticatedResponse>> Handle(LoginRequestCommand request, CancellationToken cancellationToken)
    {
        return await authService
            .Login(request.UserName, request.Password, cancellationToken);
    }
}

