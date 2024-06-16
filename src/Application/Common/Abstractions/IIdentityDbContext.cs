using CleanArchitechture.Application.Features.Identity.Models;

namespace CleanArchitechture.Application.Common.Abstractions;

public interface IIdentityDbContext
{
    DbSet<RefreshToken> RefreshTokens { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
